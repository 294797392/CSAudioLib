using GMusicCore;
using GMusicCore.Demuxer;
using MiniMusicCore;
using System;
using System.Collections.Generic;

namespace GMusicDemuxer
{
    public class MP3AudioFrameParser
    {
        public enum MPEGVersion
        {
            Mpeg1 = 0,
            Mpeg2,
            Mpeg2_5,
            Reserve
        }

        public enum Layers
        {
            LayerI = 0,
            LayerII,
            LayerIII,
            Reserve
        }

        private static log4net.ILog logger = log4net.LogManager.GetLogger("MP3StreamParser");

        /// <summary>
        /// MPEGVersion -> 采样率数组
        /// 采样率数组：
        /// 第0个元素位组合：00
        /// 第1个元素位组合：01
        /// 第2个元素位组合：10
        /// </summary>
        public static Dictionary<MPEGVersion, int[]> SamplingFrequencyMap = new Dictionary<MPEGVersion, int[]>()
        {
            { MPEGVersion.Mpeg1, new int[] { 44100, 48000, 32000 } },
            { MPEGVersion.Mpeg2, new int[] { 22050, 24000, 16000 } },
            { MPEGVersion.Mpeg2_5, new int[] { 11025, 12000, 8000 } },
        };

        /// <summary>
        /// MPEGVersion -> 比特率
        /// 比特率数组：Layer -> 比特率
        /// </summary>
        public static Dictionary<MPEGVersion, int[][]> BitRateMap = new Dictionary<MPEGVersion, int[][]>()
        {
            {
                MPEGVersion.Mpeg1,
                new int[][]
                {
                    new int[]{ 0, 32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448 , 0}, // LayerI
                    new int[]{ 0, 32, 48, 56,  64,  80,  96, 112, 128, 160, 192, 224, 256, 320, 384 , 0}, // LayerII
                    new int[]{ 0, 32, 40, 48,  56,  64,  80,  96, 112, 128, 160, 192, 224, 256, 320 , 0}  // LayerIII
                }
            },
            {
                MPEGVersion.Mpeg2,
                new int[][]
                {
                    new int[] { 0, 32, 48, 56, 64, 80, 96, 112, 128, 144, 160, 176, 192, 224, 256, 0 }, // LayerI
                    new int[] { 0,  8, 16, 24, 32, 40, 48,  56,  64,  80,  96, 112, 128, 144, 160, 0 }, // LayerII
                    new int[] { 0,  8, 16, 24, 32, 40, 48,  56,  64,  80,  96, 112, 128, 144, 160, 0 }, // LayerIII
                }
            },
            {
                MPEGVersion.Mpeg2_5,
                new int[][]
                {
                    new int[] { 0, 32, 48, 56, 64, 80, 96, 112, 128, 144, 160, 176, 192, 224, 256, 0 }, // LayerI
                    new int[] { 0,  8, 16, 24, 32, 40, 48,  56,  64,  80,  96, 112, 128, 144, 160, 0 }, // LayerII
                    new int[] { 0,  8, 16, 24, 32, 40, 48,  56,  64,  80,  96, 112, 128, 144, 160, 0 }, // LayerIII
                }
            }
        };

        public static Dictionary<MPEGVersion, int[]> SamplingPerFrame = new Dictionary<MPEGVersion, int[]>()
        {
            { MPEGVersion.Mpeg1, new int[] { 384, 1152, 1152 } },
            { MPEGVersion.Mpeg2, new int[] { 384, 1152,  576 } },
            { MPEGVersion.Mpeg2_5, new int[] { 384, 1152,  576 } },
        };

        public const int TAG_HEAD_SIZE = 10;

        /// <summary>
        /// 解析ID3V2标签固定的10个字节长度的头部
        /// </summary>
        /// <returns>
        /// 成功返回不包含标签头的tag的大小
        /// 失败返回-1
        /// </returns>
        /// 
        /// <remarks>
        /// ID3v2/file identifier       "ID3"
        /// ID3v2 version               04 00
        /// ID3v2 flags                 abcd0000
        /// ID3v2 size                  4 * 0xxxxxxx
        /// 
        /// Version or revision will never be $FF.
        /// An ID3v2 tag can be detected with the following pattern:
        /// 49 44 33 yy yy xx zz zz zz zz
        /// Where yy is less than 0xFF, xx is the 'flags' byte and zz is less than 0x80.
        /// 
        /// 参考：
        ///     http://id3.org/id3v2.4.0-structure
        ///     mplayer, demux_audio.c, id3v2_tag_size
        /// </remarks>
        public static int ParseID3V2FixedTagHead(IGMusicStream stream, byte id3v2_tag_major_version)
        {
            if (id3v2_tag_major_version == 0xFF)
            {
                logger.ErrorFormat("invalide id3v2_tag_major_version:{0}", id3v2_tag_major_version);
                return -1;
            }

            byte revision;
            if (!stream.ReadByte(out revision) || revision == 0xFF)
            {
                logger.ErrorFormat("invalide revision:{0}", revision);
                return -1;
            }

            byte flag;
            if (!stream.ReadByte(out flag))
            {
                logger.ErrorFormat("read flag failed");
                return -1;
            }

            /* 计算标标签帧的大小 */
            int size = 0;
            for (int i = 0; i < 4; i++)
            {
                byte size_byte;
                if (!stream.ReadByte(out size_byte))
                {
                    logger.ErrorFormat("read size byte failed");
                    return -1;
                }
                if (size_byte > 0x80)
                {
                    logger.ErrorFormat("invalide size byte");
                    return -1;
                }
                size = size << 7 | size_byte;
            }

            return size;
        }

        /// <summary>
        /// 找到第一帧的帧头部存储的流媒体信息，包含比特率，采样率等等
        /// MP3每一帧的帧头信息都是一样的，只要找到第一帧的帧头信息就知道了整个文件的比特率，采样率等流媒体信息
        /// 参考：
        ///     http://id3.org/mp3Frame
        ///     https://tools.ietf.org/html/rfc5219#page-3
        ///     https://www.cnblogs.com/ranson7zop/p/7655474.html
        ///     mplayer, mp3_hdr.c, mp_get_mp3_header
        /// </summary>
        public static bool FindFirstFrameHeader(IGMusicStream stream, byte id3v2_tag_major_version)
        {
            bool sync_detected = false;
            uint int32_frame_head = 0;
            Layers layer = Layers.Reserve;
            MPEGVersion mpeg_ver = MPEGVersion.Reserve;
            int sampling_frequency = 0;
            int bit_rate_kbps = 0;
            int channels = 0;
            long frame_size = 0;
            int spf = 0; // sample per frame, 每一个音频帧中，采样的个数，是一个恒定值
            uint padding = 0; // 填充

            #region 检测MP3第一帧头部同步信息（sync）,如果是同步信息，那么就说明是编码帧的帧头

            /*
             * MP3编码的每一帧的帧头的有个固定的11位的同步信息，从高位开始计算，每一位都是1
             * 这里通过“与”运算符判断帧头的前11位是否都是1（与上0xFFE00000（二进制是11111111111000000000000000000000））
             * 这11位的数据叫做同步信息（sync）
             */

            byte[] frame_head = new byte[4];
            while (true)
            {
                byte cur_byte;
                stream.ReadByte(out cur_byte);
                AppendByte(frame_head, cur_byte); /* 每次读取一个字节，并且追加到frame_head里 */
                int32_frame_head = (uint)(frame_head[0] << 24 | frame_head[1] << 16 | frame_head[2] << 8 | frame_head[3]);
                if ((int32_frame_head & 0xFFE00000) == 0xFFE00000)
                {
                    sync_detected = true;
                    break;
                }
            }

            #endregion

            if (!sync_detected)
            {
                logger.ErrorFormat("MP3 frame head , sync info not found");
                return false;
            }

            #region Layer

            /*
              |-----------------|
              | 0 0 | Reserved  |
              |-----------------|
              | 0 1 | Layer III |
              |-----------------|
              | 1 0 | Layer  II |
              |-----------------|
              | 1 1 | Layer   I |
              |-----------------|
             */

            bool bit18 = Utils.GetBit(int32_frame_head, 18);
            bool bit17 = Utils.GetBit(int32_frame_head, 17);
            if (!bit18 && !bit17)
            {
                logger.ErrorFormat("not layer-1/2/3");
                return false;
            }
            else if (!bit18 && bit17)
            {
                layer = Layers.LayerIII;
            }
            else if (bit18 && !bit17)
            {
                layer = Layers.LayerII;
            }
            else if (bit18 && bit17)
            {
                layer = Layers.LayerI;
            }

            #endregion

            #region mpeg版本

            /*
              |-----------------|
              | 0 0 | MPEG 2.5  |
              |-----------------|
              | 0 1 | Reserve   |
              |-----------------|
              | 1 0 | MPEG   2  |
              |-----------------|
              | 1 1 | MPEG   1  |
              |-----------------|
             */

            bool bit20 = Utils.GetBit(int32_frame_head, 20);
            bool bit19 = Utils.GetBit(int32_frame_head, 19);

            if (!bit20 && bit19)
            {
                logger.ErrorFormat("undefinde mpeg version");
                return false;
            }
            else if (!bit20 && !bit19)
            {
                mpeg_ver = MPEGVersion.Mpeg2_5;
            }
            else if (bit20 && !bit19)
            {
                mpeg_ver = MPEGVersion.Mpeg2;
            }
            else if (bit20 && bit19)
            {
                mpeg_ver = MPEGVersion.Mpeg1;
            }

            #endregion

            #region 采样率

            /*
              |--------------------------------|
              | bits | MPEG1 | MPEG2 | MPEG2.5 |
              |--------------------------------|
              | 0  0 | 44100 | 22050 | 11025   |
              |--------------------------------|
              | 0  1 | 48000 | 24000 | 12000   |
              |--------------------------------|
              | 1  0 | 32000 | 16000 |  8000   |
              |--------------------------------|
             */

            bool bit11 = Utils.GetBit(int32_frame_head, 11);
            bool bit10 = Utils.GetBit(int32_frame_head, 10);
            int[] sample_frequency_array = SamplingFrequencyMap[mpeg_ver];
            if (bit11 && bit10)
            {
                logger.ErrorFormat("undefinde sample_frequency");
                return false;
            }
            else if (!bit11 && !bit10)
            {
                sampling_frequency = sample_frequency_array[0];
            }
            else if (!bit11 && bit10)
            {
                sampling_frequency = sample_frequency_array[1];
            }
            else if (bit11 && !bit10)
            {
                sampling_frequency = sample_frequency_array[2];
            }

            #endregion

            #region 比特率

            /*
              |------------------------------------------------------------------|
              |      |         MPEG1               |       MPEG2/2.5(LSF)        |
              | bits |-----------------------------|-----------------------------|
              |      | LayerI | LayerII | LayerIII | LayerI | LayerII & LayerIII |
              |------------------------------------------------------------------|
              | 0000 |                            Free                           |
              |------------------------------------------------------------------|
              | 0001 |   32   |   32    |    32    |   32   |       8            |
              |------------------------------------------------------------------|
              | 0010 |   64   |   48    |    40    |   48   |      16            |
              |------------------------------------------------------------------|
              | 0011 |   96   |   56    |    48    |   56   |      24            |
              |------------------------------------------------------------------|
              | 0100 |  128   |   64    |    56    |   64   |      32            |
              |------------------------------------------------------------------|
              | 0101 |  160   |   80    |    64    |   80   |      40            |
              |------------------------------------------------------------------|
              | 0110 |  192   |   96    |    80    |   96   |      48            |
              |------------------------------------------------------------------|
              | 0111 |  224   |  112    |    96    |  112   |      56            |
              |------------------------------------------------------------------|
              | 1000 |  256   |  128    |   112    |  128   |      64            |
              |------------------------------------------------------------------|
              | 1001 |  288   |  160    |   128    |  144   |      80            |
              |------------------------------------------------------------------|
              | 1010 |  320   |  192    |   160    |  160   |      96            |
              |------------------------------------------------------------------|
              | 1011 |  352   |  224    |   192    |  176   |     112            |
              |------------------------------------------------------------------|
              | 1100 |  384   |  256    |   224    |  192   |     128            |
              |------------------------------------------------------------------|
              | 1101 |  416   |  320    |   256    |  224   |     144            |
              |------------------------------------------------------------------|
              | 1110 |  448   |  384    |   320    |  256   |     160            |
              |------------------------------------------------------------------|
              | 1111 |                            Bad                            |
              |------------------------------------------------------------------|

             */

            bool bit15 = Utils.GetBit(int32_frame_head, 15);
            bool bit14 = Utils.GetBit(int32_frame_head, 14);
            bool bit13 = Utils.GetBit(int32_frame_head, 13);
            bool bit12 = Utils.GetBit(int32_frame_head, 12);

            int[] bit_rate_array = BitRateMap[mpeg_ver][(int)layer];
            uint bit_rate_index = (int32_frame_head >> 12) & 0xF;
            bit_rate_kbps = bit_rate_array[bit_rate_index];

            #endregion

            #region 通道数

            bool bit7 = Utils.GetBit(int32_frame_head, 7);
            bool bit6 = Utils.GetBit(int32_frame_head, 6);
            channels = (((int32_frame_head >> 6) & 0x3) == 3) ? 1 : 2;

            #endregion

            #region 计算填充

            padding = (int32_frame_head >> 9) & 0x1;

            #endregion

            #region 计算帧大小

            spf = SamplingPerFrame[mpeg_ver][(int)layer];
            if (layer == Layers.LayerI)
            {
                frame_size = spf / 8 * (bit_rate_kbps * 1000) / sampling_frequency + padding * 4;
            }
            else if (layer == Layers.LayerII || layer == Layers.LayerIII)
            {
                frame_size = spf / 8 * (bit_rate_kbps * 1000) / sampling_frequency + padding;
            }

            #endregion

            return true;
        }

        private static void AppendByte(byte[] src, byte append)
        {
            int len = src.Length;
            for (int i = 0; i <= len - 2; i++)
            {
                src[i] = src[i + 1];
            }
            src[len - 1] = append;
        }
    }

    public class GMusicAudioDemuxer : IGMusicDemuxer
    {
        private const int HDR_SIZE = 4;

        public override string Name
        {
            get
            {
                return GMusicDemuxerNames.DEMUXER_AUDIO;
            }
        }

        public override int Open(IGMusicStream stream)
        {
            int read_size = 0;
            int frmt = 0, n = 0, step = 0;
            byte[] hdr = new byte[HDR_SIZE];

            if ((read_size = stream.Read(hdr, HDR_SIZE)) != HDR_SIZE)
            {
                return ResponseCode.END_OF_STREAM;
            }
            while (n < 30000 && !stream.EOF)
            {
                if (hdr[0] == 'R' && hdr[1] == 'I' && hdr[2] == 'F' && hdr[3] == 'F')
                {
                }
                else if (hdr[0] == 'I' && hdr[1] == 'D' && hdr[2] == '3')
                {
                    int id3v2_tag_size = MP3AudioFrameParser.ParseID3V2FixedTagHead(stream, hdr[3]);
                    /* 跳过固定的10个字节的id3v2头部 */
                    stream.Skip(id3v2_tag_size);
                    /* 解析MP3流媒体信息 */
                    MP3AudioFrameParser.FindFirstFrameHeader(stream, hdr[3]);
                }
            }

            return ResponseCode.SUCCESS;
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }
    }
}