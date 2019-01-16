using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using CSAudioLib.DirectSound;
using CSAudioLib;

namespace CSAudioLib.AudioCapture
{
    /// <summary>
    /// 使用DirectSound接口实现的录音器
    /// </summary>
    public class DSAudioCapture
    {
        #region 事件

        /// <summary>
        /// 录音事件
        /// 数据类型：PCM
        /// </summary>
        public event Action<byte[]> OnCaptured;

        /// <summary>
        /// 录音器发生错误时发生
        /// 参数为GetLastError的返回值
        /// </summary>
        public event Action<int> OnError;

        #endregion

        #region 实例变量

        // DirectSound对象指针
        /// <summary>
        /// IDirectSoundCapture8
        /// </summary>
        private IDirectSoundCapture8 dsc8;
        private IntPtr pdsc8;

        /// <summary>
        /// IDirectSoundCaptureBuffer8
        /// </summary>
        private IDirectSoundCaptureBuffer8 dscb8;
        private IntPtr pdscb8;

        // 通知对象句柄
        private IntPtr[] notifyHwnd_close;

        // 音频结构
        private tWAVEFORMATEX wfx;
        private IntPtr pwfx_free;
        private _DSCBUFFERDESC dsbd;

        // 当前是否正在录音
        private bool isRunning = false;

        #endregion

        #region 属性

        /// <summary>
        /// 获取录制的波形声音的格式信息
        /// </summary>
        public tWAVEFORMATEX WaveFormat
        {
            get
            {
                return this.wfx;
            }
        }

        /// <summary>
        /// 获取当前是否正在录音
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.isRunning;
            }
        }

        #endregion

        #region 公开接口

        public int Initialize()
        {
            if ((this.CreateIDirectSoundCapture8() &&
                this.CreateCaptureBuffer() &&
                this.CreateBufferNotifications()))
            {
            }

            return LibErrors.SUCCESS;
        }

        public int Start()
        {
            uint dsErr = this.dscb8.Start(LibNatives.DSCBSTART_LOOPING);
            if (dsErr != DSERR.DS_OK)
            {
                LibUtils.PrintLog("开始录音失败, DSERROR = {0}", dsErr);
                return LibErrors.DS_ERROR;
            }

            this.isRunning = true;

            Task.Factory.StartNew((state) =>
            {
                while (this.isRunning)
                {
                    // 这里需要实时获取通知对象的指针, 因为这个指针的值每隔一段时间会改变。。。
                    IntPtr lpHandles = Marshal.UnsafeAddrOfPinnedArrayElement(this.notifyHwnd_close, 0);

                    // DSLibNatives.WaitForSingleObject(this.close_notifyHwnd[0], DSLibNatives.INFINITE);
                    switch (LibNatives.WaitForMultipleObjects(LibConsts.NotifyEvents, lpHandles, false, LibNatives.INFINITE))
                    {
                        case LibNatives.WAIT_OBJECT_0:
                            {
                                (state as SynchronizationContext).Send((o) =>
                                {
                                    try
                                    {
                                        byte[] audioData = null;
                                        if (this.RecordCapturedData(0, (uint)this.wfx.nAvgBytesPerSec, out audioData) == DSERR.DS_OK)
                                        {
                                            if (this.OnCaptured != null)
                                            {
                                                this.OnCaptured(audioData);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LibUtils.PrintLog("保存音频流异常, Exception = {0}", ex);
                                    }
                                }, null);

                                LibNatives.ResetEvent(this.notifyHwnd_close[0]);
                            }
                            break;

                        case LibNatives.WAIT_OBJECT_0 + 1:
                            {
                                // 录音结束
                                LibNatives.ResetEvent(this.notifyHwnd_close[1]);

                                this.isRunning = false;
                            }
                            break;

                        case LibNatives.WAIT_FAILED:
                            {
                                int error = Marshal.GetLastWin32Error();

                                // 失败, 句柄已经被销毁
                                LibUtils.PrintLog("WAIT_FAILED, LastWin32Error = {0}", error);

                                this.isRunning = false;

                                this.Stop();

                                if (this.OnError != null)
                                {
                                    this.OnError(error);
                                }
                            }
                            break;
                    }
                }

            }, SynchronizationContext.Current);

            return LibErrors.SUCCESS;
        }

        public int Stop()
        {
            uint dsErr = this.dscb8.Stop();
            if (dsErr != DSERR.DS_OK)
            {
                LibUtils.PrintLog("停止录音失败, DSERR = {0}", dsErr);
            }

            return LibErrors.SUCCESS;
        }

        public int Release()
        {
            Marshal.FreeHGlobal(this.pwfx_free);
            Marshal.Release(this.pdscb8);
            Marshal.Release(this.pdsc8);
            //Marshal.ReleaseComObject(this.dscb8);
            //Marshal.ReleaseComObject(this.dsc8);
            Marshal.FinalReleaseComObject(this.dscb8);
            Marshal.FinalReleaseComObject(this.dsc8);

            this.pwfx_free = IntPtr.Zero;

            this.pdscb8 = IntPtr.Zero;
            this.pdsc8 = IntPtr.Zero;

            this.dscb8 = null;
            this.dsc8 = null;

            foreach (var hwnd in this.notifyHwnd_close)
            {
                LibNatives.CloseHandle(hwnd);
            }
            this.notifyHwnd_close = null;

            return LibErrors.SUCCESS;
        }

        #endregion

        #region 实例方法

        private bool CreateIDirectSoundCapture8()
        {
            uint dsErr = DSERR.DS_OK;

            dsErr = LibNatives.DirectSoundCaptureCreate8(IntPtr.Zero, out this.pdsc8, IntPtr.Zero);
            if (dsErr != DSERR.DS_OK)
            {
                LibUtils.PrintLog("DirectSoundCaptureCreate8失败, DSERROR = {0}", dsErr);
                return false;
            }

            this.dsc8 = Marshal.GetObjectForIUnknown(this.pdsc8) as IDirectSoundCapture8;

            return true;
        }

        private bool CreateCaptureBuffer()
        {
            uint dsErr = DSERR.DS_OK;

            #region 创建默认音频流格式

            this.wfx = new tWAVEFORMATEX()
            {
                nChannels = LibConsts.Channels,
                nSamplesPerSec = LibConsts.SamplesPerSec,
                wBitsPerSample = LibConsts.BitsPerSample,
                nBlockAlign = LibConsts.BlockAlign,
                nAvgBytesPerSec = LibConsts.Bps,
                cbSize = 0,
                wFormatTag = LibNatives.WAVE_FORMAT_PCM
            };

            this.pwfx_free = LibUtils.StructureToPtr(this.wfx);

            this.dsbd = new _DSCBUFFERDESC()
            {
                dwFlags = 0,
                dwSize = Marshal.SizeOf(typeof(_DSCBUFFERDESC)),
                dwReserved = 0,
                dwFXCount = 0,
                dwBufferBytes = LibConsts.BufferSize,
                lpwfxFormat = this.pwfx_free,
                lpDSCFXDesc = IntPtr.Zero
            };

            #endregion

            IntPtr pdscb;
            Guid iid_dscb8;
            dsErr = this.dsc8.CreateCaptureBuffer(ref this.dsbd, out pdscb, IntPtr.Zero); //TestInvoke2(this.free_bufferDesc, out ppDSCBuff); 
            if (dsErr == DSERR.DS_OK)
            {
                // 获取IDirectSoundCaptureBuffer8接口实例
                iid_dscb8 = new Guid(IID.IID_IDirectSoundCaptureBuffer8);
                Marshal.QueryInterface(pdscb, ref iid_dscb8, out this.pdscb8);
                Marshal.Release(pdscb);
                this.dscb8 = Marshal.GetObjectForIUnknown(this.pdscb8) as IDirectSoundCaptureBuffer8;
            }
            else
            {
                LibUtils.PrintLog("CreateCaptureBuffer失败, DSERROR = {0}", dsErr);
                return false;
            }

            return true;
        }

        private bool CreateBufferNotifications()
        {
            uint dsErr = DSERR.DS_OK;

            // 获取IDirectSoundNotify8接口
            Guid iid_dsNotify8 = new Guid(IID.IID_IDirectSoundNotify8);
            IntPtr pdsNotify8;
            IDirectSoundNotify8 dsNotify8;
            Marshal.QueryInterface(this.pdscb8, ref iid_dsNotify8, out pdsNotify8);
            dsNotify8 = Marshal.GetObjectForIUnknown(pdsNotify8) as IDirectSoundNotify8;

            try
            {
                tWAVEFORMATEX wfx;
                int pdwSizeWritten;
                dsErr = this.dscb8.GetFormat(out wfx, Marshal.SizeOf(typeof(tWAVEFORMATEX)), out pdwSizeWritten);
                if (dsErr != DSERR.DS_OK)
                {
                    LibUtils.PrintLog("GetFormat失败, DSERROR = {0}", dsErr);
                    return false;
                }

                _DSBPOSITIONNOTIFY[] rgdsbpn = new _DSBPOSITIONNOTIFY[LibConsts.NotifyEvents];
                this.notifyHwnd_close = new IntPtr[LibConsts.NotifyEvents];
                for (int i = 0; i < LibConsts.NotifyEvents; i++)
                {
                    this.notifyHwnd_close[i] = LibNatives.CreateEvent(IntPtr.Zero, true, false, null);
                }

                rgdsbpn[0].dwOffset = (uint)(wfx.nAvgBytesPerSec - 1);
                rgdsbpn[0].hEventNotify = this.notifyHwnd_close[0];

                rgdsbpn[1].dwOffset = LibNatives.DSBPN_OFFSETSTOP;
                rgdsbpn[1].hEventNotify = this.notifyHwnd_close[1];

                dsErr = dsNotify8.SetNotificationPositions(LibConsts.NotifyEvents, Marshal.UnsafeAddrOfPinnedArrayElement(rgdsbpn, 0));
                if (dsErr != DSERR.DS_OK)
                {
                    LibUtils.PrintLog("SetNotificationPositions失败, DSERROR = {0}", dsErr);
                    return false;
                }
            }
            finally
            {
                Marshal.Release(pdsNotify8);
            }

            return true;
        }

        private int RecordCapturedData(uint offset, uint dataSize, out byte[] audioData)
        {
            audioData = null;
            IntPtr pbCaptureData;
            int dwCaptureLength;
            IntPtr pbCaptureData2;
            int dwCaptureLength2;
            uint dsErr = DSERR.DS_OK;

            dsErr = this.dscb8.Lock(offset, dataSize, out pbCaptureData, out dwCaptureLength, out pbCaptureData2, out dwCaptureLength2, 0);
            if (dsErr != DSERR.DS_OK)
            {
                LibUtils.PrintLog("Lock失败, DSERROR = {0}", dsErr);
                return LibErrors.DS_ERROR;
            }

            // Unlock the capture buffer.
            this.dscb8.Unlock(pbCaptureData, dwCaptureLength, pbCaptureData2, dwCaptureLength2);

            // 拷贝音频数据
            int audioLength = dwCaptureLength + dwCaptureLength2;
            audioData = new byte[audioLength];
            Marshal.Copy(pbCaptureData, audioData, 0, dwCaptureLength);
            if (pbCaptureData2 != IntPtr.Zero)
            {
                Marshal.Copy(pbCaptureData2, audioData, dwCaptureLength, dwCaptureLength2);
            }

            return LibErrors.SUCCESS;
        }

        #endregion
    }
}