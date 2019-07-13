using ICare.Utility.Misc;
using Kagura.Player.Base;
using MediaCenter.Base;
using MediaCenter.Base.AVDemuxers;
using MediaCenter.Base.ID3;
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
    /// 
    /// mp3 编码的只是一个数字，没有位深，解码出来 pcm 想多少位都可以
    /// </summary>
    public class DemuxerAudio : AbstractAVDemuxer
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
                // 解析ID3V2格式并初始化DemuxerStream
                byte[] tag;
                int major_version, revision, flag, tag_size;
                if (!this.ParseID3V2Tag(stream, out major_version, out revision, out flag, out tag_size, out tag))
                {
                    return false;
                }
                base.ContainerHeader = new byte[header.Length + tag_size];
                Buffer.BlockCopy(header, 0, this.ContainerHeader, 0, header.Length);
                Buffer.BlockCopy(tag, 0, this.ContainerHeader, header.Length, tag_size);
            }

            this.stream = stream;

            return true;
        }

        public override bool Close()
        {
            return true;
        }

        #endregion

        protected override DemuxerPacket DemuxNextPacketCore()
        {
            long position;
            byte[] hdr, frame;
            if (!ParseMP3Frame(this.stream, out hdr, out frame, out position))
            {
                logger.ErrorFormat("NextPacket失败");
                return null;
            }

            byte[] data = new byte[hdr.Length + frame.Length];
            Buffer.BlockCopy(hdr, 0, data, 0, hdr.Length);
            Buffer.BlockCopy(frame, 0, data, hdr.Length, frame.Length);

            return new AudioDemuxPacket()
            {
                Content = frame,
                Header = hdr,
                Position = position,
                Data = data
            };
        }

        #region 实例方法

        /// <summary>
        /// 解析ID3V2标签
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="major_version"></param>
        /// <param name="revision"></param>
        /// <param name="flag"></param>
        /// <param name="size">id3v2标签的大小</param>
        /// <param name="tag">存储id3v2标签的内容</param>
        /// <returns></returns>
        private bool ParseID3V2Tag(AbstractAVStream stream, out int major_version, out int revision, out int flag, out int size, out byte[] tag)
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
             *  |            ID3              |
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
             */

            major_version = revision = flag = size = 0;
            tag = new byte[65535]; // 用64字节的临时空间用来存放tag数据

            /* 读取major version */
            major_version = stream.ReadByte();
            if (major_version == -1 || major_version == 0xFF)
            {
                return false;
            }
            tag[0] = (byte)major_version;

            /* 读取revision */
            revision = stream.ReadByte();
            if (revision == -1 || revision == 0xFF)
            {
                return false;
            }
            tag[1] = (byte)revision;

            /* 读取flags */
            if ((flag = stream.ReadByte()) == -1)
            {
                return false;
            }
            tag[2] = (byte)flag;

            /* 读取size */
            int next = 3;
            for (int i = 0; i < 4; i++)
            {
                int b = stream.ReadByte();
                if (b == -1 || (b & 0x80) == 0x80)
                {
                    return false;
                }
                size = size << 7 | (byte)b;
                tag[next + i] = (byte)b;
            }

            /* 读取id3v2的所有的帧数据 */
            if (stream.Read(tag, 7, size) != size)
            {
                return false;
            }

            // size不包含footer的大小，如果有footer，读取footer
            if ((flag & (int)ID3V2Flags.FooterPresent) == (int)ID3V2Flags.FooterPresent)
            {
                const int footer_size = 10;
                size += 10;
                if (stream.Read(tag, 7 + size, footer_size) != footer_size)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 解析MP3数据帧
        /// most of code are copied from mplayer
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="frame_size">不包括帧头的大小</param>
        /// <param name="position">当前帧在AVStream中的位置</param>
        /// <returns></returns>
        private bool ParseMP3Frame(AbstractAVStream stream, out byte[] hdr, out byte[] frame, out long position)
        {
            hdr = new byte[4];
            frame = null;
            position = -1;

            int[] mult = { 12000, 144000, 144000 };
            int[] freqs = { 44100, 48000, 32000,        // MPEG 1.0
                            22050, 24000, 16000,        // MPEG 2.0
                            11025, 12000, 8000};        // MPEG 2.5
            int bit_rate_index, mpeg, layer, sampling_frequency, padding, stereo = 0, lsf = 0, frame_size = 0, divisor = 0;
            int bitrate = 0, sample_per_frame = 0, sample_per_second = 0;

            #region 查找帧头位置

            // 帧头的4字节中高11位全部设置为1（11111111 111xxxxx xxxxxxxx xxxxxxxx），用它作为查找帧的重要依据
            if (stream.Read(hdr, 4) != 4)
            {
                return false;
            }
            uint newhead = (uint)hdr[0] << 24 | (uint)hdr[1] << 16 | (uint)hdr[2] << 8 | (uint)hdr[3];
            while ((newhead & 0xFFE00000) != 0xFFE00000)
            {
                int c = stream.ReadByte();
                if (c == -1)
                {
                    return false;
                }
                newhead = newhead << 8 | (uint)c;

                //hdr[0] = (byte)c;
                //hdr[1] = hdr[3];
                //hdr[2] = hdr[2];
                //hdr[3] = hdr[1];

                hdr[0] = hdr[1];
                hdr[1] = hdr[2];
                hdr[2] = hdr[3];
                hdr[3] = (byte)c;
            }

            #endregion

            #region 解析帧头字段

            /* mpeg版本 */
            mpeg = (int)((newhead >> 19) & 3);
            if (mpeg == 1)
            {
                logger.ErrorFormat("mpeg version error, {0}", mpeg);
                return false;
            }

            /* 采样率索引 */
            sampling_frequency = (int)((newhead >> 10) & 3);
            if (sampling_frequency == 3)
            {
                logger.ErrorFormat("sampling_frequency error, {0}", sampling_frequency);
                return false;
            }

            /* lsf: 低采样率 */
            if ((newhead & (1 << 20)) > 0)
            {
                lsf = (newhead & (1 << 19)) > 0 ? 0 : 1;
                sampling_frequency += lsf * 3;
            }
            else
            {
                lsf = 1;
                sampling_frequency += 6;
            }

            /* Layer版本 */
            layer = 4 - (int)(((newhead >> 17) & 3));
            if (layer == 0)
            {
                logger.ErrorFormat("layer error, {0}", layer);
                return false;
            }

            /* crc, 0是帧头后没有2个字节的CRC校验值，1是帧头后有2字节的CRC校验值 */
            bool crc = ((newhead << 8) & 0x80000000) == 0x80000000;

            /* 填充 */
            padding = (int)(newhead >> 9) & 0x1;

            /* 比特率，kbps，1kbit per second，每秒传输比特, 1kbit = 1000bit */
            bit_rate_index = (int)(newhead >> 12) & 0xF;
            bitrate = BitRateTable[lsf, layer - 1, bit_rate_index];
            frame_size = bitrate * mult[layer - 1];
            divisor = layer == 3 ? (freqs[sampling_frequency] << lsf) : freqs[sampling_frequency];
            frame_size /= divisor;
            frame_size += padding;
            if (layer == 1)
            {
                frame_size *= 4;
            }
            if (!crc)
            {
                frame_size += 2;
            }

            /* 声道模式 */
            stereo = (((newhead >> 6) & 0x3) == 3) ? 1 : 2;

            /* 采样率 */
            sample_per_second = freqs[sampling_frequency];

            /* 每帧采样数 */
            if (layer == 1)
            {
                sample_per_frame = 384;
            }
            else if (layer == 2)
            {
                sample_per_frame = 1152;
            }
            else if (sampling_frequency > 2) // not 1.0
            {
                sample_per_frame = 576;
            }
            else
            {
                sample_per_frame = 1152;
            }

            ///* 每帧采样数, spf, sample per frame */
            //if (layer == 3)
            //{
            //    sample_per_frame = 384;
            //}
            //else if (layer == 2)
            //{
            //    sample_per_frame = 1152;
            //}
            //else if (layer == 1)
            //{
            //    if (mpeg == 3)
            //    {
            //        sample_per_frame = 1152;
            //    }
            //    else
            //    {
            //        sample_per_frame = 576;
            //    }
            //}

            ///* 每秒采样数，采样率, sample per second / sample rate */
            //if (mpeg == 3)
            //{
            //    /* mpeg1 */
            //    if (sampling_frequency == 0)
            //    {
            //        sample_per_second = 44100;
            //    }
            //    else if (sampling_frequency == 1)
            //    {
            //        sample_per_second = 48000;
            //    }
            //    else if (sampling_frequency == 2)
            //    {
            //        sample_per_second = 32000;
            //    }
            //}
            //else if (mpeg == 2)
            //{
            //    /* mpeg2 */
            //    if (sampling_frequency == 0)
            //    {
            //        sample_per_second = 22050;
            //    }
            //    else if (sampling_frequency == 1)
            //    {
            //        sample_per_second = 24000;
            //    }
            //    else if (sampling_frequency == 2)
            //    {
            //        sample_per_second = 16000;
            //    }
            //}
            //else if (mpeg == 0)
            //{
            //    /* mpeg2.5 */
            //    if (sampling_frequency == 0)
            //    {
            //        sample_per_second = 11205;
            //    }
            //    else if (sampling_frequency == 1)
            //    {
            //        sample_per_second = 12000;
            //    }
            //    else if (sampling_frequency == 2)
            //    {
            //        sample_per_second = 8000;
            //    }
            //}

            /* 帧大小 */
            position = stream.Position - 4;
            frame = new byte[frame_size - 4]; // frame_size有帧头数据，减4个字节的帧头
            stream.Read(frame, frame.Length);

            #endregion

            return true;
        }

        #endregion
    }
}
