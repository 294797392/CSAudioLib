using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kagura.Player.Base
{
    public abstract class AudioDecoder
    {
        #region 属性

        public abstract string Name { get; }

        #endregion

        #region 公开接口

        public abstract bool Open();

        public abstract byte[] DecodeFrame(AudioPacket packet);

        public abstract bool Close();

        public abstract bool IsSupported(AudioFormats formats);

        #endregion
    }
}
