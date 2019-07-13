using Kagura.Player.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaCenter.Base
{
    /// <summary>
    /// 音视频流输入源
    /// </summary>
    public abstract class AbstractAVStream
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("AbstractAVStream");

        public abstract string Author { get; }

        public abstract List<string> Contributors { get; }

        public abstract string Name { get; }

        /// <summary>
        /// 媒体流的类型
        /// </summary>
        public abstract StreamType Type { get; }

        /// <summary>
        /// 流的总长度
        /// </summary>
        public abstract long TotalLength { get; }

        /// <summary>
        /// 获取当前流的位置
        /// </summary>
        public abstract long Position { get; }

        /// <summary>
        /// 如果剩余的音频流数据为0，返回true， 否则返回false
        /// </summary>
        public abstract bool EOF { get; }

        public abstract bool IsSupported(string uri);

        /// <summary>
        /// 打开音频流
        /// </summary>
        /// <returns></returns>
        public abstract int Open(string uri);

        /// <summary>
        /// 读取音频流
        /// </summary>
        /// <param name="dst">存储数据的缓冲区</param>
        /// <param name="count">要读取的数据大小</param>
        /// <returns>
        /// 读取的数据大小
        /// 返回0表示到流的末尾
        /// 大于0表示读取的字节数
        /// </returns>
        public abstract int Read(byte[] dst, int count);

        /// <summary>
        /// 读取流数据
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="dstOffset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public abstract int Read(byte[] dst, int dstOffset, int count);

        /// <summary>
        /// 读取一个字节的数据
        /// </summary>
        /// <returns>
        /// 读取的数据大小
        /// 返回0表示到流的末尾
        /// 大于0表示读取的字节数
        /// </returns>
        public abstract int ReadByte();

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