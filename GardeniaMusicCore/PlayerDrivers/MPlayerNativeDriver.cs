using GMusicCore.AudioSource;
using libgmusic;
using MiniMusicCore;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace GMusicCore.PlayerDrivers
{
    public class MPlayerNativeDriver : IPlayerDriver
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("MPlayerNativeDriver");

        public event Action<object, PlayerDriverEventType, double> EventCallback;

        private IntPtr mphandle;
        private int volume;
        private PlayerDriverStatus status;
        private System.Timers.Timer monitorThread;

        public int Volume { get { return this.volume; } }

        public PlayerDriverStatus Status { get { return this.status; } }

        public bool Initialize()
        {
            mplayer_opt opt;
            opt.mplayer_path = DefaultValues.MPlayerExePath;
            this.mphandle = mplayer.mplayer_create_instance(opt);
            if (this.mphandle == null)
            {
                logger.Error("实例化mplayer失败");
                return false;
            }

            this.monitorThread = new System.Timers.Timer(DefaultValues.ProgressTimerInterval);
            this.monitorThread.Elapsed += MonitorThread_Elapsed;

            return true;
        }

        public int Play(IAudioSource source)
        {
            if (string.IsNullOrEmpty(source.Uri))
            {
                logger.Error("Uri不能为空");
                return ResponseCode.INVALIDE_PARAMS;
            }

            int size = Encoding.Unicode.GetByteCount(source.Uri);
            mplayer.mplayer_open(this.mphandle, source.Uri, size);
            int ret = mplayer.mplayer_play(this.mphandle);
            if (ret != mplayer.MP_SUCCESS)
            {
                logger.ErrorFormat("mplayer_play失败:{0}", ret);
                return ResponseCode.FAILURE;
            }

            return ResponseCode.SUCCESS;
        }

        public void Stop()
        {
            mplayer.mplayer_stop(this.mphandle);
            mplayer.mplayer_close(this.mphandle);
        }

        public void Pause()
        {
            mplayer.mplayer_pause(this.mphandle);
        }

        public void Resume()
        {
            mplayer.mplayer_resume(this.mphandle);
        }

        public void Release()
        {
        }

        public int GetDuration()
        {
            return mplayer.mplayer_get_duration(this.mphandle);
        }

        public void SetVolume(int volume)
        {
        }

        #region 事件处理器

        private void MonitorThread_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.status == PlayerDriverStatus.Playing)
            {
                int progress = mplayer.mplayer_get_position(this.mphandle);
                if (this.EventCallback != null)
                {
                    this.EventCallback(this, PlayerDriverEventType.PlayProgress, progress);
                }
            }
        }

        #endregion
    }
}