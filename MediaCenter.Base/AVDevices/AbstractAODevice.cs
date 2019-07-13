using Kagura.Player.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaCenter.Base.AVDevices
{
    /// <summary>
    /// 音频输出设备
    /// </summary>
    public abstract class AbstractAODevice
    {
        public abstract string Author { get; }

        public abstract List<string> Contributors { get; }

        public abstract bool Open(WaveFormat af);

        public abstract int Play(byte[] data);

        /// <summary>
        /// 获取当前的音频缓冲区空闲的空间大小
        /// </summary>
        /// <returns></returns>
        public abstract int GetFreeSpaceSize();

        public abstract bool Close();
    }
}