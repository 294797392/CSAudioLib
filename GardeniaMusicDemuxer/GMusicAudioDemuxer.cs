using GMusicCore;
using GMusicCore.Demuxer;
using MiniMusicCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMusicDemuxer
{
    public class MP3StreamParser
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("MP3StreamParser");

        public const int TAG_HEAD_SIZE = 10;

        /// <summary>
        /// 解析固定的10个字节长度的ID3V2标签的头部
        /// </summary>
        /// <returns>
        /// 成功返回tag的大小
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
        /// 找到第一帧的帧头信息
        /// 帧头包含比特率，采样率等信息
        /// MP3每一帧的帧头信息都是一样的，只要找到第一帧的帧头信息就知道了比特率，采样率等流媒体信息
        /// </summary>
        public static bool FindFirstFrameHeader(IGMusicStream stream, byte id3v2_tag_major_version)
        {
            return true;
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
                    int id3v2_tag_size = MP3StreamParser.ParseID3V2FixedTagHead(stream, hdr[3]);
                    /* 跳过固定的10个字节的id3v2头部 */
                    stream.Skip(10); 
                    /* 解析MP3流媒体信息 */
                    MP3StreamParser.FindFirstFrameHeader(stream, hdr[3]);
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