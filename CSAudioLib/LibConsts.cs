using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAudioLib
{
    /// <summary>
    /// DirectSoundLib使用的常量定义
    /// </summary>
    public class LibConsts
    {
        /// <summary>
        /// 通知对象个数
        /// </summary>
        public static readonly int NotifyEvents = 2;

        /// <summary>
        /// 通道数量
        /// </summary>
        public static readonly short Channels = 2;

        /// <summary>
        /// 采样率
        /// </summary>
        public static readonly int SamplesPerSec = 44100;

        /// <summary>
        /// 采样位数
        /// </summary>
        public static readonly short BitsPerSample = 16;

        /// <summary>
        /// 块对齐, 每个采样的字节数
        /// </summary>
        public static readonly short BlockAlign = (short)(Channels * BitsPerSample / 8);

        /// <summary>
        /// 捕获缓冲区大小和播放缓冲区大小
        /// </summary>
        public static readonly int BufferSize = BlockAlign * SamplesPerSec;

        #region DSLibPlayer使用

        /// <summary>
        /// 缓冲区通知大小
        /// 通知大小为1秒的音频数据采样大小
        /// </summary>
        public static readonly int BUFF_NOTIFY_SIZE = BlockAlign * SamplesPerSec;

        /// <summary>
        /// 缓冲区通知次数
        /// </summary>
        public static readonly int BUFF_NOTIFY_TIMES = 4;

        /// <summary>
        /// 播放缓冲区大小
        /// 缓冲区中存放着MAX_NOTIFY_TIMES秒的音频数据
        /// </summary>
        public static readonly int PLAY_BUFF_SIZE = BUFF_NOTIFY_TIMES * BUFF_NOTIFY_SIZE;

        /// <summary>
        /// 音频每秒传输速率
        /// </summary>
        public static readonly int Bps = LibConsts.BlockAlign * LibConsts.SamplesPerSec;

        #endregion

        #region MPG123Codec使用

        /// <summary>
        /// 解码输入缓冲区大小
        /// </summary>
        public static readonly int MPG123_DECODE_IN_BUFF_SIZE = 16384;//BufferSize / 2;

        /// <summary>
        /// 解码输出缓冲区大小
        /// </summary>
        public static readonly int MPG123_DECODE_OUT_BUFF_SIZE = 32768;//BufferSize;

        #endregion
    }
}