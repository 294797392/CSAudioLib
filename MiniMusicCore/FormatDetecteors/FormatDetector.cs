using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMusicCore
{
    /// <summary>
    /// 格式探测器
    /// 用来检测音频流的编码格式
    /// </summary>
    public class FormatDetector
    {
        public static bool DetectFormat(byte[] data, out AudioFormatEnum format)
        {
            format = AudioFormatEnum.PCM;
            return true;
        }
    }
}