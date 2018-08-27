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

        /// <summary>
        /// 检测是否可以解封装
        /// 返回True：可以
        /// 返回False：不可以
        /// </summary>
        /// <returns></returns>
        public abstract bool Check();

        public abstract int Open(IGMusicStream stream);

        public abstract void Close();
    }
}