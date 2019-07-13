using Kagura.Player.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaCenter.Base.AVDemuxers
{
    /// <summary>
    /// 保存当前状态下对音视频流的解封装的信息
    /// </summary>
    public class DemuxerStream
    {
        public DemuxerStream(int hdr_size)
        {
            this.ContainerHeader = new byte[hdr_size];
        }

        public DemuxerStream(byte[] hdr, int hdr_size)
        {
            this.ContainerHeader = new byte[hdr_size];
            Buffer.BlockCopy(hdr, 0, this.ContainerHeader, 0, hdr_size);
        }

        /// <summary>
        /// 容器格式的头部数据
        /// 如果有padding，则包括padding
        /// </summary>
        public byte[] ContainerHeader { get; private set; }

        /// <summary>
        /// 从容器里抽取出来的编码后的一帧数据
        /// </summary>
        public DemuxerPacket CurrentPacket { get; set; }
    }
}