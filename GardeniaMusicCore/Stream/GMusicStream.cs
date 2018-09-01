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
        /// 流的总长度
        /// </summary>
        public abstract long TotalLength { get; }

        /// <summary>
        /// 如果剩余的音频流数据为0，返回true， 否则返回false
        /// </summary>
        public abstract bool EOF { get; }

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
        /// <param name="buff">存储数据的缓冲区</param>
        /// <param name="size">要读取的数据大小</param>
        /// <returns>读取的数据大小</returns>
        public abstract int Read(byte[] buff, int size);

        /// <summary>
        /// 读取一个字节的数据
        /// </summary>
        /// <returns></returns>
        public abstract bool ReadByte(out byte data);

        /// <summary>
        /// 从流的当前位置跳过size个字节
        /// 下次再读取就从size个字节处开始读取
        /// </summary>
        /// <param name="size">要跳过的字节数</param>
        public abstract void Skip(int size);

        /// <summary>
        /// 关闭音频流
        /// </summary>
        public abstract void Close();
    }
}