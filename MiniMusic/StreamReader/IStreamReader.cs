using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMusic.StreamReader
{
    /// <summary>
    /// 音频流读取器
    /// 为解码器提供音频流数据
    /// </summary>
    public abstract class IStreamReader
    {
        #region 事件

        public event Action<StreamInfo> OnStreamInfo;

        /// <summary>
        /// 流的位置发生改变
        /// </summary>
        public event Action<double> PositionChanged;

        /// <summary>
        /// 下载进度改变时发生
        /// 如果是负数, 就代表错误码
        /// 范围：1 - 100
        /// </summary>
        public event Action<double> DownloadProgressChanged;

        #endregion

        #region 实例变量

        protected bool isDownloading;

        #endregion

        #region 属性

        public string Uri { get; set; }

        #endregion

        #region 构造方法

        public IStreamReader()
        {
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 获取或设置音频源
        /// </summary>
        public Uri Source { get; set; }

        /// <summary>
        /// 是否正在下载音频流
        /// </summary>
        public bool IsDownloading { get { return this.isDownloading; } }

        /// <summary>
        /// 打开音频流
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public abstract int Open();

        /// <summary>
        /// 关闭音频流
        /// </summary>
        /// <returns></returns>
        public abstract int Close();

        /// <summary>
        /// 从音频流的当前位置读取数据, 并把流的位置提升读取的字节数
        /// </summary>
        /// <param name="size">要读取的数据的大小</param>
        /// <param name="buffer">保存数据的缓冲区</param>
        /// <returns></returns>
        public abstract int Read(int size, out byte[] buffer, out int bufsize);

        /// <summary>
        /// 将该流的当前位置设置为给定百分比
        /// </summary>
        /// <param name="percent">要定位到的百分比</param>
        /// <remarks>百分比取值 1 - 100</remarks>
        public abstract int Seek(double percent);

        #endregion

        #region 实例方法

        protected void NotifyDownloadProgress(double percent)
        {
            if (this.DownloadProgressChanged != null)
            {
                this.DownloadProgressChanged(percent);
            }
        }

        protected void NotifyPositionChanged(double percent)
        {
            if (this.PositionChanged != null)
            {
                this.PositionChanged(percent);
            }
        }

        #endregion
    }
}