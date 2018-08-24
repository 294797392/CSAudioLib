using GMusicCore.AudioSource;
using libgmusic;
using MiniMusicCore;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GMusicCore.PlayerDrivers
{
    public class MPlayerNativeDriver : IPlayerDriver
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("MPlayerNativeDriver");

        public event Action<object, double> Progress;

        private IntPtr mphandle;

        public bool Initialize()
        {
            this.mphandle = mplayer.mplayer_create_instance();
            if (this.mphandle == null)
            {
                logger.Error("实例化mplayer失败");
                return false;
            }

            return true;
        }

        public int Play(IAudioSource source)
        {
            if (!string.IsNullOrEmpty(source.Uri))
            {
                logger.Error("Uri不能为空");
                return ResponseCode.INVALIDE_PARAMS;
            }

            IntPtr uriPtr = Marshal.StringToHGlobalAuto(source.Uri);
            int size = Encoding.Unicode.GetByteCount(source.Uri);
            mplayer.mplayer_open(this.mphandle, uriPtr, size);

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
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }
        public void Release()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }
    }
}