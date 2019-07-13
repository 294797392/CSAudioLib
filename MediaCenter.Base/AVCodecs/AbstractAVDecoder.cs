using MediaCenter.Base.AVDemuxers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaCenter.Base.AVCodecs
{
    /// <summary>
    /// 音视频解码器
    /// </summary>
    public abstract class AbstractAVDecoder
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("AbstractAVDecoder");

        #endregion

        #region 实例变量

        // 上次解码剩余的没有播放的数据
        private List<byte> decode_data_buffer;

        #endregion

        #region 属性

        public abstract string Name { get; }

        #endregion

        public AbstractAVDecoder()
        {
            this.decode_data_buffer = new List<byte>();
        }

        #region 公开接口

        /// <summary>
        /// 使用解码库确定流真正的格式
        /// </summary>
        /// <param name="demuxer"></param>
        /// <returns></returns>
        public abstract bool Open(AbstractAVDemuxer demuxer);

        /// <summary>
        /// 解码out_size个字节数的数据
        /// 
        /// 有可能在上一次解码的时候解出来的数据比较多，一次用不完，存在缓冲区里，在下一次调用解码函数的时候继续使用
        /// </summary>
        /// <param name="out_data"></param>
        /// <param name="out_size"></param>
        /// <returns></returns>
        public bool DecodeData(out byte[] out_data, int out_size)
        {
            out_data = new byte[out_size];

            // 如果缓冲区里的数据不够用，那么继续解码，并填充到缓冲区
            if (this.decode_data_buffer.Count < out_size)
            {
                // 本次真正要解码的数据大小
                int remain_size = out_size - this.decode_data_buffer.Count; 

                byte[] decode_data;
                int decode_size;
                if (!this.DecodeDataCore(remain_size, out decode_data, out decode_size))
                {
                    return false;
                }

                // TODO：如果播放到最后，那么解码出来的数据可能比需要的数据要小


                // 把解码后的所有数据填充到缓冲区，这一个步骤可以省掉
                this.decode_data_buffer.AddRange(decode_data);
            }

            // 拷贝数据
            byte[] remain_data = this.decode_data_buffer.ToArray();
            Buffer.BlockCopy(remain_data, 0, out_data, 0, out_size);

            if (remain_data.Length > out_size)
            {
                // 如果缓冲区里的数据比需要的数据多，那么移除已经拷贝了的数据
                this.decode_data_buffer.RemoveRange(0, out_size);
            }
            else if (remain_data.Length == out_size)
            {
                // 如果缓冲区里的数据正好等于需要的数据，那么调用clear函数
                this.decode_data_buffer.Clear();
            }

            return true;
        }

        public abstract bool Close();

        public abstract bool IsSupported(AVFormats formats);

        #endregion

        #region 抽象函数

        /// <summary>
        /// 执行真正的解码操作，也是解码器组件必须要实现的接口
        /// 该函数的作用不是一帧一帧的解码，而是根据所需要的解码后的数据大小而解码
        /// 允许解码器解码出来的数据比期望的数据大，此时只要把解码后的所有数据返回和解码后的数据大小返回就可以
        /// </summary>
        /// <param name="target_size">需要解码的字节数</param>
        /// <param name="out_data">真正解出来的数据</param>
        /// <param name="out_size">
        /// 真正解出来的数据大小
        /// 注意，这个值有可能比target_size大或者小
        /// </param>
        /// <returns></returns>
        protected abstract bool DecodeDataCore(int target_size, out byte[] out_data, out int out_size);

        #endregion
    }
}
