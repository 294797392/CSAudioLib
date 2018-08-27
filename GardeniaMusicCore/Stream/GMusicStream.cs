using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMusicCore
{
    /// <summary>
    /// 提供控制音频流的接口
    /// </summary>
    public abstract class IGMusicStream
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("GMusicStream");

        public abstract string Name { get; }

        public abstract GMusicStreamType StreamType { get; }

        /// <summary>
        /// 音频源
        /// </summary>
        public MusicSource MusicSource { get; protected set; }

        /// <summary>
        /// 音乐格式
        /// </summary>
        public GMusicFormats Format { get; protected set; }

        /// <summary>
        /// 读缓冲区最大大小
        /// </summary>
        public int MaximumReadSize { get; set; }

        /// <summary>
        /// 流的总大小
        /// </summary>
        public abstract long TotalLength { get; }

        public IGMusicStream(MusicSource source)
        {
            this.MaximumReadSize = DefaultValues.GMusicStreamMaximumReadSize;
            this.MusicSource = source;
        }

        /// <summary>
        /// 此音频流是否支持protocol
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public abstract bool IsProtocolSupported(string protocol);

        /// <summary>
        /// 打开音频流
        /// </summary>
        /// <returns></returns>
        public abstract int Open();

        /// <summary>
        /// 读取音频流
        /// </summary>
        /// <returns></returns>
        public abstract bool Read();

        /// <summary>
        /// 关闭音频流
        /// </summary>
        public abstract void Close();
    }
}