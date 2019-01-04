using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kagura.Player.Base.Demuxer
{
    /// <summary>
    /// 表示一帧数据
    /// </summary>
    public class DemuxerPacket
    {
        /// <summary>
        /// 数据长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 数据在流中的位置
        /// </summary>
        public int Position { get; set; }
    }
}