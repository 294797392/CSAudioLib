using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kagura.Player.Base
{
    /// <summary>
    /// 存储一帧的媒体数据信息
    /// </summary>
    public abstract class DemuxerPacket
    {
        public int Offset { get; set; }

        public int Length { get; set; }

        public byte[] Frame { get; set; }

        public byte[] Header { get; set; }
    }

    public class AudioPacket : DemuxerPacket
    {

    }
}