using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kagura.Player.Base
{
    public class ResponseCode
    {
        public static readonly int FAILURE = -1;
        public static readonly int SUCCESS = 0;

        public static readonly int FILE_NOT_FOUND = 100;
        public static readonly int END_OF_STREAM = 101;

        /* 参数无效 */
        public static readonly int INVALIDE_PARAMS = 102; 
    }
}
