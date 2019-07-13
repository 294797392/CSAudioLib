using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaCenter.Base.AVDemuxers
{
    /// <summary>
    /// 存储一帧的媒体数据信息
    /// </summary>
    public abstract class DemuxerPacket
    {
        /// <summary>
        /// Packet在AVStream中的位置
        /// </summary>
        public long Position { get; set; }

        /// <summary>
        /// Packet的内容
        /// 一般都是帧的数据部分
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// Packet的头部内容
        /// 一般都是帧头数据
        /// </summary>
        public byte[] Header { get; set; }

        /// <summary>
        /// Header和Content合在一起的数据
        /// </summary>
        public byte[] Data { get; set; }
    }

    public class AudioDemuxPacket : DemuxerPacket
    {

    }
}