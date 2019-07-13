using Kagura.Player.Base;
using MediaCenter.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaCenter.Base.AVDemuxers
{
    /// <summary>
    /// 用来对音频流解封装的类
    /// 
    /// 解析并收集尽可能多的音视频流信息并存储到DemuxerStream类
    /// </summary>
    public abstract class AbstractAVDemuxer
    {
        #region 事件

        #endregion

        #region 属性

        public abstract string Author { get; }

        public abstract List<string> Contributors { get; }

        /// <summary>
        /// 检测出来的音频封装格式
        /// </summary>
        public AVFormats Format { get; protected set; }

        /// <summary>
        /// PCM格式信息
        /// </summary>
        public WaveFormat AudioFormat { get; private set; }

        /// <summary>
        /// Demuxer名字
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 容器格式的头部数据
        /// 如果有padding，则包括padding
        /// </summary>
        public byte[] ContainerHeader { get; protected set; }

        /// <summary>
        /// 第一帧数据
        /// </summary>
        public DemuxerPacket FirstPacket { get; protected set; }

        /// <summary>
        /// 从容器里抽取出来的编码后的一帧数据
        /// </summary>
        public DemuxerPacket CurrentPacket { get; protected set; }

        #endregion

        #region 构造方法

        public AbstractAVDemuxer()
        {
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 探测是否支持此媒体流
        /// 
        /// 1.解析ContainerHeader
        /// 2.计算第一帧的位置
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public abstract bool Open(AbstractAVStream stream);

        public abstract bool Close();

        /// <summary>
        /// 抽取下一个音视频帧
        /// </summary>
        /// <returns></returns>
        protected abstract DemuxerPacket DemuxNextPacketCore();

        #endregion

        #region 公开接口

        public bool DemuxNextPacket<T>(out T packet) where T : DemuxerPacket
        {
            packet = default(T);

            DemuxerPacket pkt = this.DemuxNextPacketCore();
            if (pkt == null)
            {
                return false;
            }

            if (this.FirstPacket == null)
            {
                this.FirstPacket = pkt;
            }

            this.CurrentPacket = pkt;

            packet = (T)pkt;

            return true;
        }

        #endregion

        #region 实例方法

        #endregion
    }
}