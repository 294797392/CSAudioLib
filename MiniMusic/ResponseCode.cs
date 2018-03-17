using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMusic
{
    public class ResponseCode
    {
        public static readonly int Success = 0;

        // AudioPlayer 1 - 100
        public static readonly int AP_DS_ERROR = 1;

        // StreamReader 100 - 500
        public static readonly int SB_FILE_NOT_FOUND = 100;
        public static readonly int SB_OPEN_FILE_FAILED = 101;

        /// <summary>
        /// 已经到达流的末尾了
        /// </summary>
        public static readonly int SB_END_OF_STREAM = 102;

        /// <summary>
        /// 无效的参数
        /// </summary>
        public static readonly int SB_INVALID_PARAM = 103;
    }
}