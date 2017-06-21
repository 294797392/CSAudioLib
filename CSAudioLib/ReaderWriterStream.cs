using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSAudioLib
{
    /// <summary>
    /// 为多线程读写的流封装的类
    /// </summary>
    public interface IReaderWriterStream
    {
        /// <summary>
        /// 获取或设置缓冲区最大大小
        /// </summary>
        int MaxBufferSize { get; set; }

        /// <summary>
        /// 向Buffer里填充数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        int PutBuffer(byte[] buffer);

        /// <summary>
        /// 从Buffer里读取数据
        /// </summary>
        /// <param name="size"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        int ReadBuffer(int size, out byte[] data);

        /// <summary>
        /// 释放Buffer
        /// </summary>
        /// <returns></returns>
        int Release();
    }

    public class ReaderWriterStreamFactory
    {
        public const int MODE_LOCK = 1;

        /// <summary>
        /// 创建一个IReaderWriterStream的实例
        /// </summary>
        /// <param name="impMode">多线程读写文件的实现方式</param>
        /// <returns></returns>
        public static IReaderWriterStream Create(int impMode = MODE_LOCK)
        {
            switch (impMode)
            {
                case MODE_LOCK:
                    return new ReaderWriterStream();

                default:
                    return null;
            }
        }
    }

    public class ReaderWriterStream : IReaderWriterStream
    {
        private static byte[] EmptyBuffer = new byte[LibConsts.BufferSize];

        private object streamLock = new object();
        private List<byte> stream = new List<byte>();

        public int MaxBufferSize { get; set; }

        public int PutBuffer(byte[] buffer)
        {
            lock (this.streamLock)
            {
                this.stream.AddRange(buffer);
            }

            return LibErrors.SUCCESS;
        }

        public int ReadBuffer(int size, out byte[] buff)
        {
            buff = null;

            lock (this.streamLock)
            {
                if (this.stream.Count < size)
                {
                    buff = EmptyBuffer;
                }
                else
                {
                    buff = new byte[size];
                    this.stream.CopyTo(0, buff, 0, buff.Length);
                    this.stream.RemoveRange(0, size);
                }
            }

            return LibErrors.SUCCESS;
        }

        public int Release()
        {
            this.stream.Clear();

            return LibErrors.SUCCESS;
        }
    }
}