using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMusicCore.PlayerDrivers
{
    public enum PlayerDriverEventType
    {
        /// <summary>
        /// 总时长（秒数）
        /// </summary>
        TotalDuration,

        /// <summary>
        /// 播放进度（秒数）
        /// </summary>
        PlayProgress,

        /// <summary>
        /// 歌曲播放结束
        /// </summary>
        EndOfPlay
    }
}