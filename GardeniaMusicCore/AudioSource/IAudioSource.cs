using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMusicCore.AudioSource
{
    public abstract class IAudioSource
    {
        /// <summary>
        /// 音频源的唯一标识, 自动生成
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// 音频Uri
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// 获取音频源类型
        /// </summary>
        public abstract AudioSourceEnum Type { get; }

        public IAudioSource()
        {
            this.ID = Guid.NewGuid().ToString();
        }

        public override string ToString()
        {
            return string.Format("{0}", Type);
        }

        public static IAudioSource Create(string uri)
        {
            return new GenericAudioSource()
            {
                Uri = uri
            };
        }
    }
}