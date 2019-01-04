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
        public event Action<DemuxerEvent, object> Event;

        public abstract string Name { get; }

        /// <summary>
        /// 探测是否支持此媒体流
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public abstract bool Open(IStream stream);

        public abstract bool Close();

        protected virtual void NotifyEvent(DemuxerEvent evt, object data)
        {
            if (this.Event != null)
            {
                this.Event(evt, data);
            }
        }
    }
}