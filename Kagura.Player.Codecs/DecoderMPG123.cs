using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kagura.Player.Base;

namespace Kagura.Player.Codecs
{
    public class DecoderMPG123 : AudioDecoder
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DecoderMPG123");

        #endregion

        #region 属性

        public override string Name => "mpg123 audio decoder";

        #endregion

        #region 公开接口

        public override bool Open()
        {
            throw new NotImplementedException();
        }

        public override byte[] DecodeFrame(DemuxerPacket packet)
        {
            throw new NotImplementedException();
        }

        public override bool Close()
        {
            throw new NotImplementedException();
        }

        public override bool IsSupported(AudioFormats formats)
        {
            switch (formats)
            {
                case AudioFormats.MP3: return true;
                default: return false;
            }
        }

        #endregion
    }
}