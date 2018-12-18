using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMusicCore.FormatDetecteors
{
    public interface IFormatDetector
    {
        bool DetectFormat(byte[] data, out AudioFormatEnum format);
    }
}