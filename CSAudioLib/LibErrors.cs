using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAudioLib
{
    public class LibErrors
    {
        /// <summary>
        /// 不支持的URI/操作
        /// </summary>
        public const int NOT_SUPPORTED = -1;

        /// <summary>
        /// 文件不存在
        /// </summary>
        public const int FILE_NOT_EXISTS = -2;

        /// <summary>
        /// 打开文件失败
        /// </summary>
        public const int OPEN_FILE_FAILED = -3;

        /// <summary>
        /// 音频流未打开
        /// </summary>
        public const int STREAM_NOT_OPENED = -4;

        /// <summary>
        /// 没有数据了
        /// </summary>
        public const int NO_DATA = -5;

        /// <summary>
        /// 发生Http错误
        /// </summary>
        public const int HTTP_ERROR = -6;

        /// <summary>
        /// 未初始化
        /// </summary>
        public const int NOT_INITIALIZED = -7;

        /// <summary>
        /// 编解码库发生错误
        /// </summary>
        public const int CODEC_ERR = -8;

        /// <summary>
        /// DirectSound错误
        /// </summary>
        public const int DS_ERROR = -9;

        /// <summary>
        /// 操作成功
        /// </summary>
        public const int SUCCESS = 0;
    }
}