using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaCenter.Base
{
    /// <summary>
    /// 音频格式枚举
    /// </summary>
    public enum AVFormats
    {
        MP3,
        AAC
    }

    public abstract class AudioFormat
    {
        public string Name { get; set; }
    }
}