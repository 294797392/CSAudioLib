using Kagura.Player.Base;
using libmpg123;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Kagura.Player.Demuxers
{
    /// <summary>
    /// http://www.cppblog.com/codejie/archive/2009/03/26/77916.aspx
    /// http://www.mpg123.de/api/
    /// http://www.guyiren.com/archives/2684
    /// http://id3.org/mp3Frame
    /// </summary>
    public class DemuxerAudio : Demuxer
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DemuxerMPG123");

        public const int SIZE_PER_READ = 4096;
        public const int PROBE_FORMATE_MAX_SIZE = 1024 * 512;

        #endregion

        #region 实例变量

        private IntPtr mpg123Handle = IntPtr.Zero;
        private IStream stream;

        #endregion

        #region 属性

        public override string Name => "mpg123 demuxer";

        #endregion

        public override bool Open(IStream stream)
        {
            this.stream = stream;

            #region 初始化mpg123句柄

            int error;
            if ((this.mpg123Handle = mpg123.mpg123_new(IntPtr.Zero, out error)) == IntPtr.Zero)
            {
                this.Close();
                logger.ErrorFormat("mpg123_new失败, {0}", error);
                return false;
            }

            if ((error = mpg123.mpg123_open_feed(this.mpg123Handle)) != mpg123.MPG123_OK)
            {
                this.Close();
                logger.ErrorFormat("mpg123_open_feed失败, {0}", error);
                return false;
            }

            #endregion

            #region 探测流格式

            int readed = 0;
            int total = 0;
            int rate, channel, encoding;
            while ((error = mpg123.mpg123_getformat(this.mpg123Handle, out rate, out channel, out encoding)) != mpg123.MPG123_OK)
            {
                if (error == mpg123.MPG123_NEED_MORE)
                {
                    if (total > PROBE_FORMATE_MAX_SIZE)
                    {
                        /* 检测了PROBE_FORMATE_MAX_SIZE字节的数据也没有检测到文件类型，返回失败 */
                        this.Close();
                        logger.ErrorFormat("mpg123_getformat失败, {0}", error);
                        return false;
                    }

                    byte[] buffer = new byte[SIZE_PER_READ];
                    if ((readed = stream.Read(buffer, buffer.Length)) == 0)
                    {
                        /* 流读完了也没有检测到数据类型，返回失败 */
                        this.Close();
                        logger.ErrorFormat("mpg123_getformat失败, {0}", error);
                        return false;
                    }
                    else
                    {
                        IntPtr data = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                        if ((error = mpg123.mpg123_feed(this.mpg123Handle, data, readed)) != mpg123.MPG123_OK)
                        {
                            /* feed数据失败，返回失败 */
                            this.Close();
                            logger.ErrorFormat("mpg123_getformat失败, {0}", error);
                            return false;
                        }
                        total += SIZE_PER_READ;
                    }
                }
                else
                {
                    /* 不认识的流格式 */
                    this.Close();
                    logger.ErrorFormat("mpg123_getformat失败, {0}", error);
                    return false;
                }
            }

            logger.InfoFormat("OpenDemuxer成功, rate={0}, channel={1}, encoding={2}", rate, channel, encoding);

            #endregion

            #region 根据流格式初始化PCM数据格式

            int bps = 0;
            if (encoding == (int)mpg123_enc_enum.MPG123_ENC_16)
            {
                bps = 16;
            }
            else if (encoding == (int)mpg123_enc_enum.MPG123_ENC_32)
            {
                bps = 32;
            }
            else
            {
                bps = 8;
                logger.WarnFormat("bps:{0}", bps);
            }

            base.AudioFormat.Channel = channel;
            base.AudioFormat.SamplesPerSec = rate;
            base.AudioFormat.BitsPerSample = bps;

            #endregion

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
    }
}