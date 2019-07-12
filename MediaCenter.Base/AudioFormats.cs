using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kagura.Player.Base
{
    /// <summary>
    /// 音频格式枚举
    /// </summary>
    public enum AudioFormats
    {
        MP3,
        AAC
    }

    public abstract class AudioFormat
    {
        public string Name { get; set; }
    }
}