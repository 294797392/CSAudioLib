using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kagura.Player.Base
{
    /// <summary>
    /// 用来对音频流解封装的类
    /// </summary>
    public abstract class Demuxer
    {
        #region 事件

        public event Action<DemuxerEvent, object> Event;

        #endregion

        #region 属性

        /// <summary>
        /// 检测出来的音频封装格式
        /// </summary>
        public AudioFormats Format { get; protected set; }

        /// <summary>
        /// PCM格式信息
        /// </summary>
        public WaveFormat AudioFormat { get; private set; }

        /// <summary>
        /// Demuxer名字
        /// </summary>
        public abstract string Name { get; }

        #endregion

        #region 构造方法

        public Demuxer()
        {
            this.AudioFormat = new WaveFormat();
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 探测是否支持此媒体流
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public abstract bool Open(IStream stream);

        /// <summary>
        /// 获取下一个音频帧信息
        /// </summary>
        /// <returns></returns>
        public abstract AudioPacket GetNextAudioPacket();

        public abstract bool Close();

        #endregion

        #region 实例方法

        protected virtual void NotifyEvent(DemuxerEvent evt, object data)
        {
            if (this.Event != null)
            {
                this.Event(evt, data);
            }
        }

        #endregion
    }
}