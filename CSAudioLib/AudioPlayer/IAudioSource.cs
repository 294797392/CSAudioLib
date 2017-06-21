using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSAudioLib.AudioPlayer
{
    public abstract class IAudioSource
    {
        /// <summary>
        /// 音频源的唯一标识, 自动生成
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// 获取音频源类型
        /// </summary>
        public abstract AudioSourceEnum Type { get; }

        public IAudioSource()
        {
            this.ID = Guid.NewGuid().ToString();
        }

        public override string ToString()
        {
            return string.Format("{0}", Type);
        }
    }

    public enum AudioSourceEnum
    {
        /// <summary>
        /// 原始采样数据文件
        /// </summary>
        Raw,

        /// <summary>
        /// 本地文件文件源
        /// </summary>
        File,

        /// <summary>
        /// 支持边下边播的Http音频源
        /// </summary>
        //Http,

        /// <summary>
        /// 实时音频
        /// </summary>
        RealTime
    }

    public class RawAudioSource : IAudioSource
    {
        public string Path { get; set; }

        /// <summary>
        /// 声道类型
        /// </summary>
        //public ChannelsEnum Channels { get; set; }

        /// <summary>
        /// 采样率
        /// </summary>
        public int SamplesPerSec { get; set; }

        /// <summary>
        /// 通道数量
        /// </summary>
        public int ChannelCount { get; set; }

        /// <summary>
        /// 采样位数
        /// </summary>
        public int BitsPerSample { get; set; }

        public override AudioSourceEnum Type
        {
            get
            {
                return AudioSourceEnum.Raw;
            }
        }
    }

    public class RealTimeAudioSource : IAudioSource
    {
        private IReaderWriterStream audioStream;

        /// <summary>
        /// 采样率
        /// </summary>
        public int SamplesPerSec { get; set; }

        /// <summary>
        /// 通道数量
        /// </summary>
        public int ChannelCount { get; set; }

        /// <summary>
        /// 采样位数
        /// </summary>
        public int BitsPerSample { get; set; }

        public override AudioSourceEnum Type
        {
            get
            {
                return AudioSourceEnum.RealTime;
            }
        }

        public RealTimeAudioSource()
        {
            this.audioStream = ReaderWriterStreamFactory.Create();
        }

        /// <summary>
        /// 向DirectSound缓冲区Put音频数据
        /// </summary>
        /// <param name="buffer">要Put的数据</param>
        /// <param name="type">Put的数据类型</param>
        public void PutBuffer(byte[] buffer)
        {
            LibUtils.PrintLog("PutBuffer, Size = {0}", buffer.Length);

            this.audioStream.PutBuffer(buffer);
        }

        /// <summary>
        /// 从缓冲区中读取数据
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public int ReadBuffer(int size, out byte[] buff)
        {
            return this.audioStream.ReadBuffer(size, out buff);
        }
    }

    //public class HttpAudioSource : IAudioSource
    //{
    //    public string Url { get; set; }

    //    public string UserAgent { get; set; }

    //    public override AudioSourceEnum Type
    //    {
    //        get
    //        {
    //            return AudioSourceEnum.Http;
    //        }
    //    }
    //}
}