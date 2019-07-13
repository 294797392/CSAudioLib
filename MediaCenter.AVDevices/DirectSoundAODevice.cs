using DirectSoundLib;
using Kagura.Player.Base;
using MediaCenter.Base.AVDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaCenter.AVDevices
{
    public class DirectSoundAODevice : AbstractAODevice
    {
        #region 事件

        #endregion

        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DirectSoundAODevice");

        #endregion

        #region 实例变量

        private IntPtr pds8;
        private IDirectSound8 ds8;

        private IntPtr pdsb8;
        private IDirectSoundBuffer8 dsb8;

        private WAVEFORMATEX wfx;
        private IntPtr pwfx_free;

        private DSBUFFERDESC dsbd;

        private IntPtr windowHandle;

        private WaveFormat audioFormat;

        private int write_offset;
        private int buffer_size;
        private int min_free_space;

        #endregion

        #region 属性

        ///// <summary>
        ///// 获取或设置音量大小
        ///// 单位：百分比
        ///// 范围：1 - 100
        ///// </summary>
        //public int Volume
        //{
        //    get
        //    {
        //        int volume;
        //        int dsErr = this.dsb8.GetVolume(out volume);
        //        if (dsErr != DSERR.DS_OK)
        //        {
        //            logger.ErrorFormat("GetVolume失败, DSERR = {0}", dsErr);
        //        }
        //        return (int)(100 * Math.Pow(10, (double)volume / 2000));
        //    }
        //    set
        //    {
        //        value = value <= 0 ? DSBVOLUME.DSBVOLUME_MIN : (int)(2000.0 * Math.Log10(value / 100.0));
        //        int dsErr = this.dsb8.SetVolume(value);
        //        if (dsErr != DSERR.DS_OK)
        //        {
        //            logger.ErrorFormat("SetVolume失败, DSERR = {0}", dsErr);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 获取当前播放进度
        ///// </summary>
        //public double Progress { get { return this.progress; } }

        #endregion

        #region 公开接口

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetDesktopWindow();

        public override bool Open(WaveFormat af)
        {
            this.windowHandle = GetDesktopWindow();

            this.audioFormat = af;

            #region 创建DirectSound对象

            int dsErr = DirectSoundHelper.DirectSoundCreate8(IntPtr.Zero, out this.pds8, IntPtr.Zero);
            if (dsErr != DSERR.DS_OK)
            {
                logger.ErrorFormat("DirectSoundCreate8失败, DSERR = {0}", dsErr);
                return false;
            }

            this.ds8 = Marshal.GetObjectForIUnknown(this.pds8) as IDirectSound8;

            dsErr = this.ds8.SetCooperativeLevel(this.windowHandle, DSSCL.DSSCL_NORMAL);
            if (dsErr != DSERR.DS_OK)
            {
                logger.ErrorFormat("SetCooperativeLevel失败, DSERR = {0}", dsErr);
                return false;
            }

            #endregion

            #region 创建音频缓冲区对象

            this.wfx = new WAVEFORMATEX()
            {
                nChannels = (short)this.audioFormat.Channel,
                nSamplesPerSec = this.audioFormat.SamplesPerSec,
                wBitsPerSample = (short)this.audioFormat.BitsPerSample,
                nBlockAlign = (short)this.audioFormat.BlockAlign,
                nAvgBytesPerSec = this.audioFormat.AvgBytesPerSec,
                cbSize = 0,
                wFormatTag = DirectSoundHelper.WAVE_FORMAT_PCM
            };

            this.pwfx_free = Utils.StructureToPtr(this.wfx);

            this.dsbd = new DSBUFFERDESC()
            {
                dwSize = Marshal.SizeOf(typeof(DSBUFFERDESC)),
                dwFlags = DSBCAPS.DSBCAPS_CTRLPOSITIONNOTIFY | DSBCAPS.DSBCAPS_GETCURRENTPOSITION2 | DSBCAPS.DSBCAPS_GLOBALFOCUS | DSBCAPS.DSBCAPS_CTRLVOLUME,
                lpwfxFormat = this.pwfx_free,
                guid3DAlgorithm = new GUID(),
                dwBufferBytes = audioFormat.AvgBytesPerSec,
                dwReserved = 0
            };
            this.buffer_size = audioFormat.AvgBytesPerSec;
            this.min_free_space = this.wfx.nBlockAlign;

            IntPtr pdsb;
            dsErr = this.ds8.CreateSoundBuffer(ref this.dsbd, out pdsb, IntPtr.Zero);
            if (dsErr != DSERR.DS_OK)
            {
                logger.ErrorFormat("CreateSoundBuffer失败, DSERR = {0}", dsErr);
                return false;
            }

            Guid iid_dsb8 = new Guid(IID.IID_IDirectSoundBuffer8);
            Marshal.QueryInterface(pdsb, ref iid_dsb8, out this.pdsb8);
            Marshal.Release(pdsb);
            this.dsb8 = Marshal.GetObjectForIUnknown(this.pdsb8) as IDirectSoundBuffer8;

            #endregion

            return true;
        }

        public override int Play(byte[] data)
        {
            uint play_offset;
            uint write_cursor;
            int space;
            int len = data.Length;

            // make sure we have enough space to write data
            this.dsb8.GetCurrentPosition(out play_offset, out write_cursor);
            space = Convert.ToInt32(this.buffer_size - (write_offset - play_offset));
            if (space > buffer_size) space -= buffer_size; // write_offset < play_offset
            if (space < len) len = space;

            return this.WriteDataToBuffer(data, len);
        }

        public override int GetFreeSpaceSize()
        {
            uint play_offset, write_cursor;
            this.dsb8.GetCurrentPosition(out play_offset, out write_cursor);
            int space = Convert.ToInt32(this.buffer_size - (this.write_offset - play_offset));

            if (space > this.buffer_size)
            {
                space -= this.buffer_size;
            }
            if (space < min_free_space)
            {
                return 0;
            }
            return space - min_free_space;
        }

        public override bool Close()
        {
            int dsErr = this.dsb8.Stop();
            if (dsErr != DSERR.DS_OK)
            {
                logger.ErrorFormat("Stop失败, DSERR = {0}", dsErr);
            }

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

            return true;
        }

        #endregion

        #region 实例方法

        //private bool CreateBufferNotifications()
        //{
        //    int dsErr = DSERR.DS_OK;

        //    // 获取IDirectSoundNotify8接口
        //    Guid iid_dsNotify8 = new Guid(IID.IID_IDirectSoundNotify8);
        //    IntPtr pdsNotify8;
        //    IDirectSoundNotify8 dsNotify8;
        //    Marshal.QueryInterface(this.pdsb8, ref iid_dsNotify8, out pdsNotify8);
        //    dsNotify8 = Marshal.GetObjectForIUnknown(pdsNotify8) as IDirectSoundNotify8;

        //    try
        //    {
        //        uint written;
        //        WAVEFORMATEX wfx;
        //        dsErr = this.dsb8.GetFormat(out wfx, (uint)Marshal.SizeOf(typeof(WAVEFORMATEX)), out written);
        //        if (dsErr != DSERR.DS_OK)
        //        {
        //            logger.ErrorFormat("GetFormat失败, DSERR = {0}", dsErr);
        //            return false;
        //        }

        //        this.rgdsbpn = new DSBPOSITIONNOTIFY[Consts.BUFF_NOTIFY_TIMES];
        //        this.notifyHwnd_close = new IntPtr[Consts.BUFF_NOTIFY_TIMES];
        //        for (int idx = 0; idx < Consts.BUFF_NOTIFY_TIMES; idx++)
        //        {
        //            IntPtr pHandle = Natives.CreateEvent(IntPtr.Zero, false, false, null);
        //            this.notifyHwnd_close[idx] = pHandle;
        //            this.rgdsbpn[idx].dwOffset = (uint)(Consts.BUFF_NOTIFY_SIZE * idx);
        //            this.rgdsbpn[idx].hEventNotify = pHandle;
        //        }

        //        dsErr = dsNotify8.SetNotificationPositions(Consts.BUFF_NOTIFY_TIMES, Marshal.UnsafeAddrOfPinnedArrayElement(rgdsbpn, 0));
        //        if (dsErr != DSERR.DS_OK)
        //        {
        //            logger.ErrorFormat("SetNotificationPositions失败, DSERROR = {0}", dsErr);
        //            return false;
        //        }
        //    }
        //    finally
        //    {
        //        Marshal.Release(pdsNotify8);
        //    }

        //    return true;
        //}

        private int WriteDataToBuffer(byte[] data, int len)
        {
            IntPtr audioPtr1, audioPtr2;
            int audioBytes1, audioBytes2, dataLength = len;

            uint dsErr = (uint)this.dsb8.Lock(this.write_offset, dataLength, out audioPtr1, out audioBytes1, out audioPtr2, out audioBytes2, 0);
            if (dsErr == DSERR.DSERR_BUFFERLOST)
            {
                this.dsb8.Restore();
                dsErr = (uint)this.dsb8.Lock(this.write_offset, dataLength, out audioPtr1, out audioBytes1, out audioPtr2, out audioBytes2, 0);
                if (dsErr != DSERR.DS_OK)
                {
                    logger.ErrorFormat("Lock失败, DSERR = {0}", dsErr);
                    return -1;
                }
            }

            Marshal.Copy(data, 0, audioPtr1, (int)audioBytes1);


            if (audioPtr2 != IntPtr.Zero) Marshal.Copy(data, (int)audioBytes1, audioPtr2, (int)audioBytes2);
            this.write_offset += audioBytes1 + audioBytes2;
            if (this.write_offset >= this.buffer_size) write_offset = audioBytes2;

            dsErr = (uint)this.dsb8.Unlock(audioPtr1, audioBytes1, audioPtr2, audioBytes2);
            if (dsErr != DSERR.DS_OK)
            {
                logger.ErrorFormat("Unlock失败, DSERR = {0}", dsErr);
                return -1;
            }

            int status;
            this.dsb8.GetStatus(out status);
            if ((status & DSBSTATUS.DSBSTATUS_PLAYING) != DSBSTATUS.DSBSTATUS_PLAYING)
            {
                this.dsb8.Play(0, 0, DSBPLAY.DSBPLAY_LOOPING);
            }

            return audioBytes1 + audioBytes2;
        }

        #endregion
    }
}