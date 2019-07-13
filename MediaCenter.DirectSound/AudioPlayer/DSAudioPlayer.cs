using CSAudioLib.AudioPlayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSAudioLib.DirectSound
{
    /// <summary>
    /// 使用DirectSound接口实现的音频播放器
    /// </summary>
    public class DSAudioPlayer
    {
        #region 事件

        /// <summary>
        /// 播放状态改变发生
        /// 第一个参数：状态
        /// 第二个参数：如果状态是Error, 那么是LastWin32Error, 其他状态都是0
        /// </summary>
        public event Action<DSLibPlayerStatus, int> StatusChanged;

        ///// <summary>
        ///// 播放进度改变事件
        ///// 总进度为百分之百
        ///// </summary>
        //public event Action<double> ProgressChanged;

            /// <summary>
            /// 下载百分比改变
            /// 取值：1 - 100
            /// </summary>
        public event Action<double> DownloadProgressChanged;

        #endregion

        #region 类变量

        #endregion

        #region 实例变量

        private IntPtr pds8;
        private IDirectSound8 ds8;

        private IntPtr pdsb8;
        private IDirectSoundBuffer8 dsb8;

        private tWAVEFORMATEX wfx;
        private IntPtr pwfx_free;

        private _DSBUFFERDESC dsbd;

        // 通知句柄
        private _DSBPOSITIONNOTIFY[] rgdsbpn;
        private IntPtr[] notifyHwnd_close;

        // 是否正在播放
        private bool isPlaying = false;

        private DSLibPlayerStatus status;

        private IAudioStream stream;
        private double progress; // 当前播放进度

        private bool isInit;

        #endregion

        #region 属性

        /// <summary>
        /// 获取音频源
        /// </summary>
        public IAudioSource Source { get { return this.stream.Source; } }

        public IntPtr WindowHandle { get; set; }

        public DSLibPlayerStatus Status { get { return this.status; } }

        /// <summary>
        /// 获取或设置音量大小
        /// 单位：百分比
        /// 范围：1 - 100
        /// </summary>
        public int Volume
        {
            get
            {
                int volume;
                uint dsErr = this.dsb8.GetVolume(out volume);
                if (dsErr != DSERR.DS_OK)
                {
                    LibUtils.PrintLog("GetVolume失败, DSERR = {0}", dsErr);
                }
                return (int)(100 * Math.Pow(10, (double)volume / 2000));
            }
            set
            {
                value = value <= 0 ? DSBVOLUME.DSBVOLUME_MIN : (int)(2000.0 * Math.Log10(value / 100.0));
                uint dsErr = this.dsb8.SetVolume(value);
                if (dsErr != DSERR.DS_OK)
                {
                    LibUtils.PrintLog("SetVolume失败, DSERR = {0}", dsErr);
                }
            }
        }

        ///// <summary>
        ///// 获取当前播放进度
        ///// </summary>
        //public double Progress { get { return this.progress; } }

        #endregion

        #region 公开接口

        public int Initialize()
        {
            if (this.isInit)
            {
                return LibErrors.SUCCESS;
            }

            this.isInit = true;

            if ((this.CreateIDirectSound8(this.WindowHandle) &&
                this.CreateSecondaryBuffer() &&
                this.CreateBufferNotifications()))
            {

            }

            return LibErrors.DS_ERROR;
        }

        public int Release()
        {
            if (!this.isInit)
            {
                return LibErrors.SUCCESS;
            }

            this.isInit = false;

            Marshal.FreeHGlobal(this.pwfx_free);
            Marshal.Release(this.pdsb8);
            Marshal.Release(this.pds8);
            //Marshal.ReleaseComObject(this.dsb8);
            //Marshal.ReleaseComObject(this.ds8);
            Marshal.FinalReleaseComObject(this.dsb8);
            Marshal.FinalReleaseComObject(this.ds8);

            this.pwfx_free = IntPtr.Zero;

            this.pdsb8 = IntPtr.Zero;
            this.pds8 = IntPtr.Zero;

            this.dsb8 = null;
            this.ds8 = null;

            foreach (var item in notifyHwnd_close)
            {
                LibNatives.CloseHandle(item);
            }
            this.notifyHwnd_close = null;
            this.rgdsbpn = null;

            return LibErrors.SUCCESS;
        }

        /// <summary>
        /// 打开音频源
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public int Open(IAudioSource source)
        {
            if (this.stream != null)
            {
                this.Close();
            }

            this.stream = IAudioStream.Create(source);
            if (this.stream == null)
            {
                LibUtils.PrintLog("不支持的音频源:{0}", source);
                return LibErrors.NOT_SUPPORTED;
            }

            int ret = this.stream.Open();
            if (ret != LibErrors.SUCCESS)
            {
                return ret;
            }

            this.stream.PlaybackProgressChanged += this.Stream_PlaybackProgressChanged;
            this.stream.DownloadProgressChanged += this.Stream_DownloadProgressChanged;

            return LibErrors.SUCCESS;
        }

        /// <summary>
        /// 关闭音频源
        /// </summary>
        /// <returns></returns>
        public int Close()
        {
            if (this.stream != null)
            {
                this.stream.PlaybackProgressChanged -= this.Stream_PlaybackProgressChanged;
                this.stream.DownloadProgressChanged -= this.Stream_DownloadProgressChanged;
                this.stream.Close();
                this.stream = null;
            }

            return LibErrors.SUCCESS;
        }

        public int Play()
        {
            if (!this.isInit)
            {
                return LibErrors.NOT_INITIALIZED;
            }

            if (this.stream == null)
            {
                return LibErrors.STREAM_NOT_OPENED;
            }

            this.dsb8.SetCurrentPosition(0);
            uint dsErr = this.dsb8.Play(0, 0, DSBPLAY.DSBPLAY_LOOPING);
            if (dsErr != DSERR.DS_OK)
            {
                LibUtils.PrintLog("Play失败, DSERR = {0}", dsErr);
                return LibErrors.DS_ERROR;
            }

            this.isPlaying = true;

            this.NotifyStatusChanged(DSLibPlayerStatus.Playing, 0);

            uint offset = (uint)LibConsts.BUFF_NOTIFY_SIZE;

            Task.Factory.StartNew((state) =>
            {
                while (this.isPlaying)
                {
                    IntPtr lpHandles = Marshal.UnsafeAddrOfPinnedArrayElement(this.notifyHwnd_close, 0);

                    uint notifyIdx = LibNatives.WaitForMultipleObjects(LibConsts.BUFF_NOTIFY_TIMES, lpHandles, false, LibNatives.INFINITE);
                    if ((notifyIdx >= LibNatives.WAIT_OBJECT_0) && (notifyIdx <= LibNatives.WAIT_OBJECT_0 + LibConsts.BUFF_NOTIFY_TIMES))
                    {
                        if (!this.isPlaying)
                        {
                            // 通知是异步的,在调用了Stop之后, 如果收到通知的速度比音频流Seek(0)的速度慢（也就是说先重置了音频流，然后又收到了一次通知）, 有可能会再次读取一次数据
                            break;
                        }

                        this.HandleNotification(offset, LibConsts.BUFF_NOTIFY_SIZE, state as SynchronizationContext);

                        offset += (uint)LibConsts.BUFF_NOTIFY_SIZE;
                        offset %= (uint)(LibConsts.BUFF_NOTIFY_SIZE * LibConsts.BUFF_NOTIFY_TIMES);

                        //Console.WriteLine("dwOffset = {0}, offset = {1}", this.rgdsbpn[notifyIdx].dwOffset, offset);
                    }
                    else if (notifyIdx == LibNatives.WAIT_FAILED)
                    {
                        int winErr = Marshal.GetLastWin32Error();

                        LibUtils.PrintLog("等待信号失败, LastWin32Error = {0}", winErr);
                        (state as SynchronizationContext).Send((o) =>
                        {
                            this.Stop();
                        }, null);
                        this.NotifyStatusChanged(DSLibPlayerStatus.Error, winErr);
                    }
                }

                LibUtils.PrintLog("跳出循环");

            }, SynchronizationContext.Current);

            return LibErrors.SUCCESS;
        }

        public int Stop()
        {
            if (!this.isInit)
            {
                return LibErrors.NOT_INITIALIZED;
            }

            if (this.stream == null)
            {
                return LibErrors.STREAM_NOT_OPENED;
            }

            this.isPlaying = false;

            uint dsErr = this.dsb8.Stop();
            if (dsErr != DSERR.DS_OK)
            {
                LibUtils.PrintLog("Stop失败, DSERR = {0}", dsErr);
            }

            this.stream.Seek(0);

            LibNatives.SetEvent(this.notifyHwnd_close[0]);

            return LibErrors.SUCCESS;
        }

        //public int Pause()
        //{
        //    return DSERR.DS_OK;
        //}

        //public int Restore()
        //{
        //    return DSERR.DS_OK;
        //}

        #endregion

        #region 实例方法

        private bool CreateIDirectSound8(IntPtr hwnd)
        {
            uint dsErr = LibNatives.DirectSoundCreate8(IntPtr.Zero, out this.pds8, IntPtr.Zero);
            if (dsErr != DSERR.DS_OK)
            {
                LibUtils.PrintLog("DirectSoundCreate8失败, DSERR = {0}", dsErr);
                return false;
            }

            this.ds8 = Marshal.GetObjectForIUnknown(this.pds8) as IDirectSound8;

            dsErr = this.ds8.SetCooperativeLevel(hwnd, DSSCL.DSSCL_NORMAL);
            if (dsErr != DSERR.DS_OK)
            {
                LibUtils.PrintLog("SetCooperativeLevel失败, DSERR = {0}", dsErr);
                return false;
            }

            return true;
        }

        private bool CreateSecondaryBuffer()
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

            this.dsbd = new _DSBUFFERDESC()
            {
                dwSize = Marshal.SizeOf(typeof(_DSBUFFERDESC)),
                dwFlags = DSBCAPS.DSBCAPS_CTRLPOSITIONNOTIFY | DSBCAPS.DSBCAPS_GETCURRENTPOSITION2 | DSBCAPS.DSBCAPS_GLOBALFOCUS | DSBCAPS.DSBCAPS_CTRLVOLUME,
                lpwfxFormat = this.pwfx_free,
                guid3DAlgorithm = new _GUID(),
                dwBufferBytes = LibConsts.PLAY_BUFF_SIZE,
                dwReserved = 0
            };

            #endregion

            IntPtr pdsb;
            dsErr = this.ds8.CreateSoundBuffer(ref this.dsbd, out pdsb, IntPtr.Zero);
            if (dsErr != DSERR.DS_OK)
            {
                LibUtils.PrintLog("CreateSoundBuffer失败, DSERR = {0}", dsErr);
                return false;
            }

            Guid iid_dsb8 = new Guid(IID.IID_IDirectSoundBuffer8);
            Marshal.QueryInterface(pdsb, ref iid_dsb8, out this.pdsb8);
            Marshal.Release(pdsb);
            this.dsb8 = Marshal.GetObjectForIUnknown(this.pdsb8) as IDirectSoundBuffer8;

            return true;
        }

        private bool CreateBufferNotifications()
        {
            uint dsErr = DSERR.DS_OK;

            // 获取IDirectSoundNotify8接口
            Guid iid_dsNotify8 = new Guid(IID.IID_IDirectSoundNotify8);
            IntPtr pdsNotify8;
            IDirectSoundNotify8 dsNotify8;
            Marshal.QueryInterface(this.pdsb8, ref iid_dsNotify8, out pdsNotify8);
            dsNotify8 = Marshal.GetObjectForIUnknown(pdsNotify8) as IDirectSoundNotify8;

            try
            {
                uint written;
                tWAVEFORMATEX wfx;
                dsErr = this.dsb8.GetFormat(out wfx, (uint)Marshal.SizeOf(typeof(tWAVEFORMATEX)), out written);
                if (dsErr != DSERR.DS_OK)
                {
                    LibUtils.PrintLog("GetFormat失败, DSERR = {0}", dsErr);
                    return false;
                }

                this.rgdsbpn = new _DSBPOSITIONNOTIFY[LibConsts.BUFF_NOTIFY_TIMES];
                this.notifyHwnd_close = new IntPtr[LibConsts.BUFF_NOTIFY_TIMES];
                for (int idx = 0; idx < LibConsts.BUFF_NOTIFY_TIMES; idx++)
                {
                    IntPtr pHandle = LibNatives.CreateEvent(IntPtr.Zero, false, false, null);
                    this.notifyHwnd_close[idx] = pHandle;
                    this.rgdsbpn[idx].dwOffset = (uint)(LibConsts.BUFF_NOTIFY_SIZE * idx);
                    this.rgdsbpn[idx].hEventNotify = pHandle;
                }

                dsErr = dsNotify8.SetNotificationPositions(LibConsts.BUFF_NOTIFY_TIMES, Marshal.UnsafeAddrOfPinnedArrayElement(rgdsbpn, 0));
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

        private bool WriteDataToBuffer(uint offset, byte[] data)
        {
            IntPtr audioPtr1, audioPtr2;
            uint audioBytes1, audioBytes2, dataLength = (uint)data.Length;

            uint dsErr = this.dsb8.Lock(offset, dataLength, out audioPtr1, out audioBytes1, out audioPtr2, out audioBytes2, 0);
            if (dsErr == DSERR.DSERR_BUFFERLOST)
            {
                this.dsb8.Restore();
                dsErr = this.dsb8.Lock(offset, dataLength, out audioPtr1, out audioBytes1, out audioPtr2, out audioBytes2, 0);
                if (dsErr != DSERR.DS_OK)
                {
                    LibUtils.PrintLog("Lock失败, DSERR = {0}", dsErr);
                    return false;
                }
            }

            if (data != null && data.Length > 0)
            {
                Marshal.Copy(data, 0, audioPtr1, (int)audioBytes1);
                if (audioBytes2 > 0 && audioPtr2 != IntPtr.Zero)
                {
                    Marshal.Copy(data, (int)audioBytes1, audioPtr2, (int)audioBytes2);
                }
            }
            else
            {
                // 填充空数据
                //DSLibNatives.memset(audioPtr1, 0, audioBytes1);
                //if (audioPtr2 != IntPtr.Zero)
                //{
                //    DSLibNatives.memset(audioPtr2, 0, audioBytes2);
                //}
            }

            dsErr = this.dsb8.Unlock(audioPtr1, audioBytes1, audioPtr2, audioBytes2);
            if (dsErr != DSERR.DS_OK)
            {
                LibUtils.PrintLog("Unlock失败, DSERR = {0}", dsErr);
                return false;
            }

            return true;
        }

        private void HandleNotification(uint offset, int playSize, SynchronizationContext ctx)
        {
            byte[] buffer;
            int ret = this.stream.Read(playSize, out buffer);
            if (ret == LibErrors.SUCCESS)
            {
                #region 读取音频流成功

                ctx.Send((o) =>
                {
                    //DSLibUtils.PrintLog("播放缓冲区数据, 大小:{0}字节", buffer.Length);

                    try
                    {
                        if (this.WriteDataToBuffer(offset, buffer))
                        {
                        }
                    }
                    catch (Exception ex)
                    {
                        LibUtils.PrintLog("播放音频流异常, Exception = {0}", ex);
                    }
                }, null);

                #endregion
            }
            else if (ret == LibErrors.NO_DATA)
            {
                #region 没数据了, 播放完毕

                ctx.Send((o) =>
                {
                    LibUtils.PrintLog("缓冲区播放完毕, 停止播放");
                    this.Stop();
                    this.NotifyStatusChanged(DSLibPlayerStatus.Stopped, 0);
                }, null);

                #endregion
            }
        }

        private void NotifyStatusChanged(DSLibPlayerStatus status, int errCode)
        {
            this.status = status;

            if (this.StatusChanged != null)
            {
                this.StatusChanged(status, errCode);
            }
        }

        #endregion

        #region 事件处理器

        private void Stream_DownloadProgressChanged(double progress)
        {
            if (this.DownloadProgressChanged != null)
            {
                this.DownloadProgressChanged(progress);
            }
        }

        private void Stream_PlaybackProgressChanged(double progress)
        {
        }

        #endregion
    }

    public enum DSLibPlayerStatus
    {
        /// <summary>
        /// 空闲状态, 未播放
        /// </summary>
        Stopped,

        /// <summary>
        /// 正在播放
        /// </summary>
        Playing,

        /// <summary>
        /// 播放过程中发生错误
        /// </summary>
        Error
    }
}