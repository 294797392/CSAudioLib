using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMusicCore.AudioStream
{
    /// <summary>
    /// 提供控制音频流的接口
    /// </summary>
    public abstract class IAudioStream
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("IAudioStream");

        /// <summary>
        /// 音频流选项
        /// </summary>
        public Dictionary<string, object> Options { get; private set; }

        /// <summary>
        /// 支持的播放协议
        /// </summary>
        public abstract List<string> SupportProtocols { get; }

        public IAudioStream()
        {
            this.Options = new Dictionary<string, object>();
        }

        /// <summary>
        /// 打开音频流
        /// </summary>
        /// <returns></returns>
        public abstract bool Open();

        /// <summary>
        /// 读取音频流
        /// </summary>
        /// <returns></returns>
        public abstract bool Read();
    }
}