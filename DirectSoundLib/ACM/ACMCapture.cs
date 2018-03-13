using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace CSAudioLib.ACM
{
    /// <summary>
    /// 使用waveIn函数制作的录音机
    /// </summary>
    public class ACMCapture
    {
        #region 常量

        /// <summary>
        /// 通知对象个数
        /// </summary>
        private const int NotifyEvents = 2;

        /// <summary>
        /// 通道数量
        /// </summary>
        private const int Channels = 2;

        /// <summary>
        /// 采样率
        /// </summary>
        private const uint SamplesPerSec = 44100;

        /// <summary>
        /// 采样位数
        /// </summary>
        private const int BitsPerSample = 16;

        /// <summary>
        /// 块对齐, 每个采样的字节数
        /// </summary>
        private static readonly ushort BlockAlign = Channels * BitsPerSample / 8;

        /// <summary>
        /// 捕获缓冲区大小
        /// </summary>
        private static readonly int BufferSize = (int)(BlockAlign * SamplesPerSec);

        #endregion

        #region 事件

        public event Action<byte[]> AudioCaptured;

        #endregion

        #region 实例变量

        private waveNatives.waveInProcDlg waveInProcDlg;

        private IntPtr free_pwfx;
        private IntPtr free_pAudioData;
        /// <summary>
        /// WaveHeader指针
        /// </summary>
        private IntPtr free_pwh;
        private int whSize;

        /// <summary>
        /// WaveIn Hwnd
        /// </summary>
        private IntPtr hwi;

        private bool isStop = true;

        #endregion

        #region 公开接口

        public int Initialize()
        {
            #region waveInOpen

            WAVEFORMATEX  wfx = new WAVEFORMATEX ()
            {
                nChannels = Channels,
                nSamplesPerSec = SamplesPerSec,
                wBitsPerSample = BitsPerSample,
                nBlockAlign = BlockAlign,
                nAvgBytesPerSec = (uint)(BlockAlign * SamplesPerSec),
                cbSize = 0,
                wFormatTag = 1
            };
            this.free_pwfx = LibUtils.StructureToPtr(wfx);
            this.waveInProcDlg = new waveNatives.waveInProcDlg(this.waveInProc);
            int retValue = waveNatives.waveInOpen(out this.hwi, waveNatives.WAVE_MAPPER, this.free_pwfx, this.waveInProcDlg, 0, waveNatives.WAVE_FORMAT_DIRECT | waveNatives.CALLBACK_FUNCTION);
            if (retValue != MMSYSERR.MMSYSERR_NOERROR)
            {
                LibUtils.PrintLog("waveInOpen失败, MMSYSERROR = {0}", retValue);
                return retValue;
            }

            #endregion

            #region waveInPrepareHeader

            wavehdr_tag wh = new wavehdr_tag()
            {
                lpData = this.free_pAudioData = Marshal.AllocHGlobal((int)(BlockAlign * SamplesPerSec)),
                dwBufferLength = (BlockAlign * SamplesPerSec),
                dwFlags = 0x00000002
            };
            this.whSize = Marshal.SizeOf(typeof(wavehdr_tag));
            this.free_pwh = LibUtils.StructureToPtr(wh);
            retValue = waveNatives.waveInPrepareHeader(hwi, this.free_pwh, (uint)this.whSize);
            if (retValue != MMSYSERR.MMSYSERR_NOERROR)
            {
                LibUtils.PrintLog("waveInPrepareHeader失败, MMSYSERROR = {0}", retValue);
                return retValue;
            }

            #endregion

            #region waveInAddBuffer

            retValue = waveNatives.waveInAddBuffer(hwi, this.free_pwh, (uint)this.whSize);
            if (retValue != MMSYSERR.MMSYSERR_NOERROR)
            {
                LibUtils.PrintLog("waveInAddBuffer失败, MMSYSERROR = {0}", retValue);
                return retValue;
            }

            #endregion

            return MMSYSERR.MMSYSERR_NOERROR;
        }

        public int Start()
        {
            this.isStop = false;

            int retValue = waveNatives.waveInStart(this.hwi);
            if (retValue != MMSYSERR.MMSYSERR_NOERROR)
            {
                LibUtils.PrintLog("waveInStart失败, MMSYSERROR = {0}", retValue);
                return retValue;
            }

            return MMSYSERR.MMSYSERR_NOERROR;
        }

        public int Stop()
        {
            this.isStop = true;

            int retValue = waveNatives.waveInStop(this.hwi);
            if (retValue != MMSYSERR.MMSYSERR_NOERROR)
            {
                LibUtils.PrintLog("waveInStop失败, MMSYSERROR = {0}", retValue);
            }

            return MMSYSERR.MMSYSERR_NOERROR;
        }

        public int Release()
        {
            waveNatives.waveInUnprepareHeader(this.hwi, this.free_pwh, (uint)this.whSize);
            waveNatives.waveInReset(this.hwi);
            waveNatives.waveInClose(this.hwi);

            Marshal.FreeHGlobal(this.free_pAudioData);
            Marshal.FreeHGlobal(this.free_pwfx);
            Marshal.FreeHGlobal(this.free_pwh);

            return MMSYSERR.MMSYSERR_NOERROR;
        }

        #endregion

        #region 事件处理器

        private void waveInProc(IntPtr hwi, uint uMsg, uint dwInstance, uint dwParam1, uint dwParam2)
        {
            switch (uMsg)
            {
                case (uint)waveNatives.uMsgEnum.WIM_OPEN:
                    LibUtils.PrintLog("OPEN");
                    break;

                case (uint)waveNatives.uMsgEnum.WIM_DATA:
                    if (this.isStop)
                    {
                        break;
                    }

                    wavehdr_tag hdr = (wavehdr_tag)Marshal.PtrToStructure(this.free_pwh, typeof(wavehdr_tag));

                    // 处理音频数据
                    {
                        byte[] buffer = new byte[hdr.dwBytesRecorded];
                        Marshal.Copy(hdr.lpData, buffer, 0, buffer.Length);

                        if (this.AudioCaptured != null)
                        {
                            this.AudioCaptured(buffer);
                        }
                    }

                    int retValue = waveNatives.waveInAddBuffer(hwi, this.free_pwh, (uint)this.whSize);
                    if (retValue != MMSYSERR.MMSYSERR_NOERROR)
                    {
                        LibUtils.PrintLog("waveInAddBuffer失败, MMSYSERROR = {0}", retValue);
                    }

                    break;

                case (uint)waveNatives.uMsgEnum.WIM_CLOSE:
                    LibUtils.PrintLog("CLOSE");
                    break;
            }
        }

        #endregion
    }
}