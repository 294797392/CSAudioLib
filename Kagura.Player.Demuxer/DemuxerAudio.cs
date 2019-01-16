using Kagura.Player.Base;
using libmpg123;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Kagura.Player.Demuxers
{
    /// <summary>
    /// MP3:
    /// http://www.cppblog.com/codejie/archive/2009/03/26/77916.aspx
    /// https://www.cnblogs.com/ranson7zop/p/7655474.html
    /// http://www.mpg123.de/api/
    /// http://www.guyiren.com/archives/2684
    /// http://id3.org/mp3Frame
    /// http://id3.org/id3v2.4.0-structure
    /// </summary>
    public class DemuxerAudio : Demuxer
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DemuxerMPG123");

        public const int SIZE_PER_READ = 4096;
        public const int PROBE_FORMATE_MAX_SIZE = 1024 * 512;

        private static int[,,] BitRateTable = new int[2, 3, 16]
        {
            {
                { 0,32,64,96,128,160,192,224,256,288,320,352,384,416,448,0 },
                { 0,32,48,56, 64, 80, 96,112,128,160,192,224,256,320,384,0 },
                { 0,32,40,48, 56, 64, 80, 96,112,128,160,192,224,256,320,0 }
            },
            {
                { 0,32,48,56,64,80,96,112,128,144,160,176,192,224,256,0 },
                { 0,8,16,24,32,40,48,56,64,80,96,112,128,144,160,0 },
                { 0,8,16,24,32,40,48,56,64,80,96,112,128,144,160,0 }
            }
        };

        #endregion

        #region 实例变量

        //private IntPtr mpg123Handle = IntPtr.Zero;
        private IStream stream;
        private AudioFormats format;

        /// <summary>
        /// 第一帧的位置
        /// </summary>
        private long first_frame_pos = -1;

        #endregion

        #region 属性

        public override string Name => "mpg123 demuxer";

        #endregion

        #region 公开接口

        //public override bool Open(IStream stream)
        //{
        //    this.stream = stream;

        //    #region 初始化mpg123句柄

        //    int error;
        //    if ((this.mpg123Handle = mpg123.mpg123_new(IntPtr.Zero, out error)) == IntPtr.Zero)
        //    {
        //        this.Close();
        //        logger.ErrorFormat("mpg123_new失败, {0}", error);
        //        return false;
        //    }

        //    if ((error = mpg123.mpg123_open_feed(this.mpg123Handle)) != mpg123.MPG123_OK)
        //    {
        //        this.Close();
        //        logger.ErrorFormat("mpg123_open_feed失败, {0}", error);
        //        return false;
        //    }

        //    #endregion

        //    #region 探测流格式

        //    int readed = 0;
        //    int total = 0;
        //    int rate, channel, encoding;
        //    while ((error = mpg123.mpg123_getformat(this.mpg123Handle, out rate, out channel, out encoding)) != mpg123.MPG123_OK)
        //    {
        //        if (error == mpg123.MPG123_NEED_MORE)
        //        {
        //            if (total > PROBE_FORMATE_MAX_SIZE)
        //            {
        //                /* 检测了PROBE_FORMATE_MAX_SIZE字节的数据也没有检测到文件类型，返回失败 */
        //                this.Close();
        //                logger.ErrorFormat("mpg123_getformat失败, {0}", error);
        //                return false;
        //            }

        //            byte[] buffer = new byte[SIZE_PER_READ];
        //            if ((readed = stream.Read(buffer, buffer.Length)) == 0)
        //            {
        //                /* 流读完了也没有检测到数据类型，返回失败 */
        //                this.Close();
        //                logger.ErrorFormat("mpg123_getformat失败, {0}", error);
        //                return false;
        //            }
        //            else
        //            {
        //                IntPtr data = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
        //                if ((error = mpg123.mpg123_feed(this.mpg123Handle, data, readed)) != mpg123.MPG123_OK)
        //                {
        //                    /* feed数据失败，返回失败 */
        //                    this.Close();
        //                    logger.ErrorFormat("mpg123_getformat失败, {0}", error);
        //                    return false;
        //                }
        //                total += SIZE_PER_READ;
        //            }
        //        }
        //        else
        //        {
        //            /* 不认识的流格式 */
        //            this.Close();
        //            logger.ErrorFormat("mpg123_getformat失败, {0}", error);
        //            return false;
        //        }
        //    }

        //    logger.InfoFormat("OpenDemuxer成功, rate={0}, channel={1}, encoding={2}", rate, channel, encoding);

        //    #endregion

        //    #region 根据流格式初始化PCM数据格式

        //    int bps = 0;
        //    if (encoding == (int)mpg123_enc_enum.MPG123_ENC_16)
        //    {
        //        bps = 16;
        //    }
        //    else if (encoding == (int)mpg123_enc_enum.MPG123_ENC_32)
        //    {
        //        bps = 32;
        //    }
        //    else
        //    {
        //        bps = 8;
        //        logger.WarnFormat("bps:{0}", bps);
        //    }

        //    base.AudioFormat.Channel = channel;
        //    base.AudioFormat.SamplesPerSec = rate;
        //    base.AudioFormat.BitsPerSample = bps;

        //    #endregion

        //    return true;
        //}

        public override bool Open(IStream stream)
        {
            byte[] header = new byte[3];
            if (stream.Read(header, header.Length) < header.Length)
            {
                return false;
            }

            if (header[0] == 'R' && header[1] == 'I' && header[2] == 'F')
            {
            }
            else if (header[0] == 'I' && header[1] == 'D' && header[2] == '3')
            {
                /*
                 *  ID3V2格式：
                 *  
                 *  +-----------------------------+
                 *  |       Header(10 bytes)      |
                 *  +-----------------------------+
                 *  |       Extended Header       |
                 *  | (variable length, OPTIONAL) |
                 *  +-----------------------------+
                 *  |   Frames(variable length)   |
                 *  +-----------------------------+
                 *  |           Padding           |
                 *  | (variable length, OPTIONAL) |
                 *  +-----------------------------+
                 *  |  Footer(10 bytes, OPTIONAL) |
                 *  +-----------------------------+
                 */

                #region 解析Header

                int major_version, revision, flag, tag_size;
                if (!this.ParseID3V2(stream, out major_version, out revision, out flag, out tag_size))
                {
                    return false;
                }

                #endregion

                #region 根据解析Header得到的tag_size，跳过tag_size字节，准备查找第一帧的位置

                /* tag_size不包含footer的大小，如果有footer，要加上10个字节，因为footer的大小是10个字节 */
                if ((flag & (int)ID3V2Flags.FooterPresent) == (int)ID3V2Flags.FooterPresent)
                {
                    tag_size += 10;
                }
                stream.Skip(tag_size);

                #endregion

                #region 解析MP3第一帧

                if (!this.ParseMP3Frame(stream))
                {
                    return false;
                }

                #endregion
            }

            return true;
        }

        /// <summary>
        /// 获取下一个音频帧信息
        /// </summary>
        /// <returns></returns>
        public override AudioPacket GetNextAudioPacket()
        {
            throw new NotImplementedException();
        }

        public override bool Close()
        {
            return true;
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 解析ID3V2结构的头部数据
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="major_version"></param>
        /// <param name="revision"></param>
        /// <param name="flag"></param>
        /// <param name="tag_size"></param>
        /// <returns></returns>
        private bool ParseID3V2(IStream stream, out int major_version, out int revision, out int flag, out int tag_size)
        {
            /*
             *  +-----------------------------+
             *  |    major_version(1 bytes)   |
             *  +-----------------------------+
             *  |      revision(1 bytes)      |
             *  +-----------------------------+
             *  |       flags(1 bytes)        |
             *  +-----------------------------+
             *  |        size(4 bytes)        |
             *  +-----------------------------+
             */

            major_version = revision = flag = tag_size = 0;

            major_version = stream.ReadByte();
            if (major_version == -1 || major_version == 0xFF || major_version == 0xFF)
            {
                return false;
            }

            revision = stream.ReadByte();
            if (revision == -1 || revision == 0xFF || revision == 0xFF)
            {
                return false;
            }

            if ((flag = stream.ReadByte()) == -1)
            {
                return false;
            }

            for (int i = 0; i < 4; i++)
            {
                int b = stream.ReadByte();
                if (b == -1 || (b & 0x80) == 0x80)
                {
                    return false;
                }
                tag_size = tag_size << 7 | (byte)b;
            }

            return true;
        }

        /// <summary>
        /// 解析MP3数据帧
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private bool ParseMP3Frame(IStream stream)
        {
            int bit_rate_index, mpeg, layer, sampling_frequency, padding, stereo = 0, lsf = 0;
            int bitrate = 0, sample_per_frame = 0, sample_per_second = 0, padding_size = 0, frame_size;

            #region 查找帧头位置

            byte[] hdr_bytes = new byte[4];
            if (stream.Read(hdr_bytes, 4) != 4)
            {
                return false;
            }
            uint newhead = (uint)hdr_bytes[0] << 24 | (uint)hdr_bytes[1] << 16 | (uint)hdr_bytes[2] << 8 | (uint)hdr_bytes[3];
            while ((newhead & 0xFFE00000) != 0xFFE00000)
            {
                int c = stream.ReadByte();
                if (c == -1)
                {
                    return false;
                }
                newhead = newhead << 8 | (uint)c;
            }

            if (this.first_frame_pos == -1)
            {
                this.first_frame_pos = stream.Position - 4;
            }

            #endregion

            #region 解析帧头字段

            /* mpeg版本 */
            mpeg = (int)((newhead >> 19) & 3);
            if (mpeg == 1)
            {
                return false;
            }
            lsf = mpeg == 0 ? 1 : 0;

            /* Layer版本 */
            layer = (int)(((newhead >> 17) & 3));
            if (layer == 0)
            {
                return false;
            }

            /* 采样率索引 */
            sampling_frequency = (int)((newhead >> 10) & 3);
            if (sampling_frequency == 3)
            {
                return false;
            }

            /* 比特率，kbps，1kbit per second，每秒传输比特, 1kbit = 1000bit */
            bit_rate_index = (int)(newhead >> 12) & 0xF;
            bitrate = BitRateTable[lsf, layer - 1, bit_rate_index];

            /* 填充 */
            padding = (int)(newhead >> 9) & 0x1;
            padding_size = layer == 3 ? 4 : 1;

            /* 声道模式 */
            stereo = (((newhead >> 6) & 0x3) == 3) ? 1 : 2;

            /* 每帧的采样数, spf, sample per frame */
            if (layer == 3)
            {
                sample_per_frame = 384;
            }
            else if (layer == 2)
            {
                sample_per_frame = 1152;
            }
            else if (layer == 1)
            {
                if (mpeg == 3)
                {
                    sample_per_frame = 1152;
                }
                else
                {
                    sample_per_frame = 576;
                }
            }

            /* 每秒采样数，采样频率 */
            if (mpeg == 3)
            {
                /* mpeg1 */
                if (sampling_frequency == 0)
                {
                    sample_per_second = 44100;
                }
                else if (sampling_frequency == 1)
                {
                    sample_per_second = 48000;
                }
                else if (sampling_frequency == 2)
                {
                    sample_per_second = 32000;
                }
            }
            else if (mpeg == 2)
            {
                /* mpeg2 */
                if (sampling_frequency == 0)
                {
                    sample_per_second = 22050;
                }
                else if (sampling_frequency == 1)
                {
                    sample_per_second = 24000;
                }
                else if (sampling_frequency == 2)
                {
                    sample_per_second = 16000;
                }
            }
            else if (mpeg == 0)
            {
                /* mpeg2.5 */
                if (sampling_frequency == 0)
                {
                    sample_per_second = 11205;
                }
                else if (sampling_frequency == 1)
                {
                    sample_per_second = 12000;
                }
                else if (sampling_frequency == 2)
                {
                    sample_per_second = 8000;
                }
            }

            /* 帧大小 */
            frame_size = ((sample_per_frame / 8 * bitrate * 1000) / sample_per_second) + padding_size;

            #endregion

            return true;
        }

        #endregion
    }
}
