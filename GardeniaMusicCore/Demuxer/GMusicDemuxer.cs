using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMusicCore
{
    /// <summary>
    /// 用来对音频流解封装的类
    /// </summary>
    public abstract class IGMusicDemuxer
    {
        public abstract string Name { get; }

        public abstract int Open(IGMusicStream stream);

        public abstract void Close();
    }
}