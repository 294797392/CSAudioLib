using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMusicCore.AudioPlayer
{
    public enum PlayStatus
    {
        /// <summary>
        /// 空闲状态, 未播放
        /// </summary>
        Stopped,

        /// <summary>
        /// 正在播放
        /// </summary>
        Playing,

        /// <summary>
        /// 播放过程中发生错误
        /// </summary>
        Error
    }
}