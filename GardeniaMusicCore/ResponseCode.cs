using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMusicCore
{
    public class ResponseCode
    {
        public static readonly int FAILURE = -1;
        public static readonly int SUCCESS = 0;

        public static readonly int FILE_NOT_FOUND = 100;


        /* 参数无效 */
        public static readonly int INVALIDE_PARAMS = 102; 


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