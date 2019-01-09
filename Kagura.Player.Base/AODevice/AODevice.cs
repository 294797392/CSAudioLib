using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kagura.Player.Base.AODevice
{
    public abstract class AODevice
    {
        public abstract bool Open(WaveFormat af);

        public abstract bool Play(byte[] data);

        /// <summary>
        /// 获取当前的音频缓冲区空闲的空间大小
        /// </summary>
        /// <returns></returns>
        public abstract int GetFreeSpaceSize();

        public abstract bool Close();
    }
}