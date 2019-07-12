using Kagura.Player.Base;
using MediaCenter.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MediaCenter.AVDemuxers
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
        private AbstractAVStream stream;
        private AudioFormats format;

        /// <summary>
        /// 第一帧的位置
        /// </summary>
        private long current_frame_position = -1;

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

        public override bool Open(AbstractAVStream stream)
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
                #region 解析ID3V2标签

                int major_version, revision, flag, tag_size;
                if (!this.ParseID3V2Tag(stream, out major_version, out revision, out flag, out tag_size))
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
            int frame_size;
            if (!this.ParseMP3Frame(this.stream, out frame_size))
            {
                return null;
            }

            AudioPacket packet = new AudioPacket();
            return packet;
        }

        public override bool Close()
        {
            return true;
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 解析ID3V2标签
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="major_version"></param>
        /// <param name="revision"></param>
        /// <param name="flag"></param>
        /// <param name="size">id3v2标签的大小，不包含footer的大小，如果flags里有footer标志位，则标签大小是tag_size+10</param>
        /// <returns></returns>
        private bool ParseID3V2Tag(AbstractAVStream stream, out int major_version, out int revision, out int flag, out int size)
        {

            /*
             *  ID3V2标签结构：
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
             *  
             *  10个字节的Header结构：
             *  +-----------------------------+
             *  |    major_version(1 bytes)   |
             *  +-----------------------------+
             *  |      revision(1 bytes)      |
             *  +-----------------------------+
             *  |       flags(1 bytes)        |
             *  +-----------------------------+
             *  |        size(4 bytes)        |
             *  +-----------------------------+
             *  
             * The ID3v2 tag size is the sum of the byte length of the extended
             * header, the padding and the frames after unsynchronisation. If a
             * footer is present this equals to ('total size' - 20) bytes, otherwise
             * ('total size' - 10) bytes.
             * 
             */

            major_version = revision = flag = size = 0;

            /* 读取major version */
            major_version = stream.ReadByte();
            if (major_version == -1 || major_version == 0xFF || major_version == 0xFF)
            {
                return false;
            }

            /* 读取revision */
            revision = stream.ReadByte();
            if (revision == -1 || revision == 0xFF || revision == 0xFF)
            {
                return false;
            }

            /* 读取flags */
            if ((flag = stream.ReadByte()) == -1)
            {
                return false;
            }

            /* 读取size */
            for (int i = 0; i < 4; i++)
            {
                int b = stream.ReadByte();
                if (b == -1 || (b & 0x80) == 0x80)
                {
                    return false;
                }
                size = size << 7 | (byte)b;
            }

            /* 读取id3v2的所有的帧数据 */
            byte[] frames = new byte[size];
            if (stream.Read(frames, frames.Length) != size)
            {
                return false;
            }

            /* size不包含footer的大小，如果有footer，读取footer */
            if ((flag & (int)ID3V2Flags.FooterPresent) == (int)ID3V2Flags.FooterPresent)
            {
                byte[] footerBytes = new byte[10];
                if (stream.Read(footerBytes, footerBytes.Length) != footerBytes.Length)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 解析MP3数据帧
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="frame_size">不包括帧头的大小</param>
        /// <returns></returns>
        private bool ParseMP3Frame(AbstractAVStream stream, out int frame_size)
        {
            int bit_rate_index, mpeg, layer, sampling_frequency, padding, stereo = 0, lsf = 0;
            int bitrate = 0, sample_per_frame = 0, sample_per_second = 0, padding_size = 0;
            frame_size = 0;

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
