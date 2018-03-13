using CSAudioLib.DirectSound;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSAudioLib.AudioPlayer
{
    public abstract class IAudioStream
    {
        #region 事件

        public event Action<double> PlaybackProgressChanged;

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

        /// <summary>
        /// 总时长
        /// 单位：秒
        /// </summary>
        public int TotalLength { get; protected set; }

        #endregion

        #region 构造方法

        public IAudioStream(IAudioSource source)
        {
            this.Source = source;
        }

        #endregion

        #region 公开接口

        public IAudioSource Source { get; private set; }

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
        public abstract int Read(int size, out byte[] buffer);

        /// <summary>
        /// 将该流的当前位置设置为给定百分比
        /// </summary>
        /// <param name="percent">要定位到的百分比</param>
        /// <remarks>百分比取值 1 - 100</remarks>
        public abstract int Seek(double percent);

        public abstract tWAVEFORMATEX GetFormat();

        #endregion

        #region 实例方法

        protected void NotifyDownloadProgress(double percent)
        {
            if (this.DownloadProgressChanged != null)
            {
                this.DownloadProgressChanged(percent);
            }
        }

        protected void NotifyPlaybackProgress(double percent)
        {
            if (this.PlaybackProgressChanged != null)
            {
                this.PlaybackProgressChanged(percent);
            }
        }

        #endregion

        public static IAudioStream Create(IAudioSource source)
        {
            switch (source.Type)
            {
                case AudioSourceEnum.Raw:
                    return new RawAudioStream(source);

                case AudioSourceEnum.RealTime:
                    return new RealTimeAudioStream(source);

                default:
                    return null;
            }
        }
    }

    public class RawAudioStream : IAudioStream
    {
        private RawAudioSource source;
        private FileStream stream;

        public RawAudioStream(IAudioSource source) :
            base(source)
        {
            this.source = source as RawAudioSource;
        }

        public override int Open()
        {
            if (!File.Exists(this.source.Path))
            {
                LibUtils.PrintLog("AudioSource.Open失败, 文件不存在, Uri = {0}", this.source.Path);
                return LibErrors.FILE_NOT_EXISTS;
            }

            base.isDownloading = true;

            try
            {
                this.stream = File.Open(this.source.Path, FileMode.Open);
            }
            catch (Exception ex)
            {
                LibUtils.PrintLog("AudioSource.Open失败, 打开文件失败, 异常信息:{0}", ex);
                return LibErrors.OPEN_FILE_FAILED;
            }

            base.isDownloading = false;

            return LibErrors.SUCCESS;
        }

        public override int Close()
        {
            if (this.stream != null)
            {
                this.stream.Close();
                this.stream.Dispose();
                this.stream = null;
            }

            return LibErrors.SUCCESS;
        }

        public override int Read(int size, out byte[] buffer)
        {
            byte[] buf = new byte[size];
            int readSize = this.stream.Read(buf, 0, buf.Length);
            if (readSize > 0)
            {
                buffer = buf;
                return LibErrors.SUCCESS;
            }
            else
            {
                buffer = null;
                return LibErrors.NO_DATA;
            }
        }

        public override int Seek(double percent)
        {
            int target = (int)(this.stream.Length * (percent / 100));

            this.stream.Seek(target, SeekOrigin.Begin);

            return LibErrors.SUCCESS;
        }

        public override tWAVEFORMATEX GetFormat()
        {
            short blockAlign = LibUtils.GetBlockAlign(this.source.ChannelCount, this.source.BitsPerSample);

            return new tWAVEFORMATEX()
            {
                nChannels = (short)this.source.ChannelCount,
                nSamplesPerSec = this.source.SamplesPerSec,
                wBitsPerSample = (short)this.source.BitsPerSample,
                nBlockAlign = blockAlign,
                nAvgBytesPerSec = LibUtils.GetBytesPerSec(blockAlign, this.source.SamplesPerSec),
                cbSize = 0,
                wFormatTag = LibNatives.WAVE_FORMAT_PCM
            };
        }
    }

    public class RealTimeAudioStream : IAudioStream
    {
        private RealTimeAudioSource source;

        public RealTimeAudioStream(IAudioSource source) :
            base(source)
        {
            this.source = source as RealTimeAudioSource;
        }

        public override int Open()
        {
            base.isDownloading = true;

            return LibErrors.SUCCESS;
        }

        public override int Close()
        {
            base.isDownloading = false;

            return LibErrors.SUCCESS;
        }

        public override int Read(int size, out byte[] buffer)
        {
            return this.source.ReadBuffer(size, out buffer);
        }

        public override int Seek(double percent)
        {
            return LibErrors.NOT_SUPPORTED;
        }

        public override tWAVEFORMATEX GetFormat()
        {
            short blockAlign = LibUtils.GetBlockAlign(this.source.ChannelCount, this.source.BitsPerSample);

            return new tWAVEFORMATEX()
            {
                nChannels = (short)this.source.ChannelCount,
                nSamplesPerSec = this.source.SamplesPerSec,
                wBitsPerSample = (short)this.source.BitsPerSample,
                nBlockAlign = blockAlign,
                nAvgBytesPerSec = LibUtils.GetBytesPerSec(blockAlign, this.source.SamplesPerSec),
                cbSize = 0,
                wFormatTag = LibNatives.WAVE_FORMAT_PCM
            };
        }
    }

    //public class HttpAudioStream : IAudioStream
    //{
    //    #region 常量

    //    private const int DEFAULT_READ_BUFF_SIZE = 8192;
    //    private const int DEFAULT_DOWNLOAD_THREAD_NUM = 1;

    //    #endregion

    //    #region 实例变量

    //    private HttpAudioSource httpSource;

    //    private HttpWebRequest httpReq;
    //    private WebResponse response;
    //    private Stream responseStream;

    //    private long totalBytes;  // 总字节数
    //    private int dlBytes; // 已经下载了的字节数
    //    private int playbackBytes; // 回放了的字节数

    //    // 多线程分片下载
    //    private int dlThreadNum;  // 下载线程数量
    //    private int totalSlice;
    //    private int currentSlice;

    //    // 接收数据缓冲区
    //    private int recvBufferSize;  // 接收数据缓冲区大小
    //    private byte[] recvBuff;

    //    private IReaderWriterStream audioStream;

    //    #endregion

    //    #region 公开接口

    //    public HttpAudioStream(IAudioSource source) :
    //        base(source)
    //    {
    //        this.httpSource = source as HttpAudioSource;
    //    }

    //    public override int Open()
    //    {
    //        this.recvBufferSize = DEFAULT_READ_BUFF_SIZE;
    //        this.dlThreadNum = DEFAULT_DOWNLOAD_THREAD_NUM;

    //        base.isDownloading = true;
    //        this.audioStream = ReaderWriterStreamFactory.Create();

    //        this.httpReq = WebRequest.Create(this.httpSource.Url) as HttpWebRequest;
    //        this.httpReq.UserAgent = this.httpSource.UserAgent;
    //        this.httpReq.BeginGetResponse(new AsyncCallback(this.GetResponseCallback), null);

    //        return DSLibErrors.SUCCESS;
    //    }

    //    public override int Close()
    //    {
    //        if (this.httpReq != null)
    //        {
    //            this.httpReq.Abort();
    //        }

    //        if (this.response != null)
    //        {
    //            this.response.Close();
    //        }

    //        if (this.responseStream != null)
    //        {
    //            this.responseStream.Close();
    //            this.responseStream.Dispose();
    //        }

    //        if (this.audioStream != null)
    //        {
    //            this.audioStream.Release();
    //        }

    //        return DSLibErrors.SUCCESS;
    //    }

    //    public override int Read(int size, out byte[] buffer)
    //    {
    //        int ret = this.audioStream.ReadBuffer(size, out buffer);
    //        if (ret == DSLibErrors.SUCCESS)
    //        {
    //            //this.playbackBytes += size;

    //            //double percent = Math.Round(((double)this.playbackBytes / this.totalBytes), 4) * 100;

    //            //base.NotifyPlaybackProgress(percent);

    //            //DSLibUtils.PrintLog("播放进度:{0}%", percent);
    //        }

    //        return ret;
    //    }

    //    #endregion

    //    #region 实例方法

    //    private void HandleException(Exception ex, string msg)
    //    {
    //        base.isDownloading = false;

    //        if (ex is WebException)
    //        {
    //            var webErr = ex as WebException;
    //            // 如果请求是手动调用Close终止的, 此时不通知Error
    //            if (webErr.Status != WebExceptionStatus.RequestCanceled)
    //            {
    //                base.NotifyDownloadProgress(DSLibErrors.HTTP_ERROR);
    //            }
    //            DSLibUtils.PrintLog("{0}, StatusCode = {1}", msg, webErr.Status);
    //        }
    //        else
    //        {
    //            DSLibUtils.PrintLog("{0}, 异常信息:{1}", msg, ex);
    //            base.NotifyDownloadProgress(DSLibErrors.HTTP_ERROR);
    //        }
    //    }

    //    private void GetResponseCallback(IAsyncResult iar)
    //    {
    //        try
    //        {
    //            this.response = this.httpReq.EndGetResponse(iar);
    //        }
    //        catch (Exception ex)
    //        {
    //            this.HandleException(ex, "Http GetResponse异常");
    //            return;
    //        }

    //        this.totalBytes = response.ContentLength;
    //        this.totalSlice = (int)Math.Ceiling((double)(this.totalBytes / this.recvBufferSize));
    //        this.responseStream = response.GetResponseStream();
    //        this.currentSlice = this.dlThreadNum;
    //        this.recvBuff = new byte[this.recvBufferSize];
    //        this.dlBytes = 0;
    //        this.playbackBytes = 0;

    //        for (int idx = 0; idx < this.dlThreadNum; idx++)
    //        {
    //            this.responseStream.BeginRead(this.recvBuff, 0, this.recvBuff.Length, new AsyncCallback(this.ReadCallback), idx);
    //        }
    //    }

    //    private void ReadCallback(IAsyncResult iar)
    //    {
    //        int size = 0;

    //        try
    //        {
    //            size = this.responseStream.EndRead(iar);
    //            this.dlBytes += size;
    //        }
    //        catch (Exception ex)
    //        {
    //            this.HandleException(ex, "EndRead异常");
    //            return;
    //        }

    //        if (size > 0)
    //        {
    //            byte[] buffer = this.recvBuff;

    //            if (size < this.recvBufferSize)
    //            {
    //                // 有可能下载的数据大小比缓冲区小
    //                buffer = new byte[size];
    //                Array.Copy(this.recvBuff, buffer, buffer.Length);
    //            }

    //            byte[] pcm;
    //            if (IAudioExtractor.GetExtractor(base.source.Format).Decode(buffer, out pcm) == DSLibErrors.SUCCESS)
    //            {
    //                this.audioStream.PutBuffer(pcm);
    //            }
    //            else
    //            {
    //                base.isDownloading = false;

    //                base.NotifyDownloadProgress(DSLibErrors.CODEC_ERR);
    //                return;
    //            }

    //            double percent = Math.Round(((double)this.dlBytes / this.totalBytes), 4) * 100;

    //            base.NotifyDownloadProgress(percent);

    //            DSLibUtils.PrintLog("下载进度:{0}%", percent);

    //            //DSLibUtils.PrintLog("总共大小:{0}, 已下载:{1}", this.totalSize, this.totalDlBytes);

    //            // 测试用
    //            //System.Threading.Thread.Sleep(20);

    //            try
    //            {
    //                // AudioStream被Close了, 此时下载线程还未工作完成, 会出现responseStream为null的情况
    //                this.responseStream.BeginRead(this.recvBuff, 0, this.recvBuff.Length, new AsyncCallback(this.ReadCallback), iar.AsyncState);
    //            }
    //            catch (Exception ex)
    //            {
    //                this.HandleException(ex, "BeginRead异常");
    //            }
    //        }
    //        else
    //        {
    //            base.isDownloading = false;

    //            base.NotifyDownloadProgress(100);
    //        }
    //    }

    //    public override int Seek(double percent)
    //    {
    //        return DSLibErrors.NOT_SUPPORTED;
    //    }

    //    #endregion
    //}
}