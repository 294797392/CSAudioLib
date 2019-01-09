using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kagura.Player.Base
{
    /// <summary>
    /// 存储PCM波形文件的信息
    /// </summary>
    public class WaveFormat
    {
        /// <summary>
        /// 通道数量
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        /// 每秒采样率
        /// </summary>
        public int SamplesPerSec { get; set; }

        /// <summary>
        /// 每个采样的位数
        /// </summary>
        public int BitsPerSample { get; set; }

        /// <summary>
        /// 块大小
        /// 对采样数据进行操作的时候，一次性读取BlockAlign字节的数据
        /// </summary>
        public int BlockAlign
        {
            get
            {
                return this.BitsPerSample / 8 * this.Channel;
            }
        }

        /// <summary>
        /// 每秒播放的字节数
        /// </summary>
        public int AvgBytesPerSec
        {
            get
            {
                return this.SamplesPerSec * (this.BitsPerSample / 8) * this.Channel;
            }
        }
    }
}
