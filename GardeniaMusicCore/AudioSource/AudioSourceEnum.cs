using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMusicCore.AudioSource
{
    public enum AudioSourceEnum
    {
        Generic,

        /// <summary>
        /// 原始采样数据文件
        /// </summary>
        Raw,

        /// <summary>
        /// 本地文件文件源
        /// </summary>
        File,

        /// <summary>
        /// 支持边下边播的Http音频源
        /// </summary>
        //Http,

        /// <summary>
        /// 实时音频
        /// </summary>
        RealTime
    }
}
