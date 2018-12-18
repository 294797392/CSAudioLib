using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMusicCore
{
    public enum MusicStreamType
    {
        /// <summary>
        /// 网络流
        /// same as FILE but no seeking (for net/stdin)
        /// </summary>
        Stream,

        /// <summary>
        /// 文件流
        /// read from seekable file
        /// </summary>
        File
    }
}
