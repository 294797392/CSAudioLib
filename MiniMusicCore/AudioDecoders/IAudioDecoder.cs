using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMusicCore
{
    public interface IAudioDecoder
    {
        int Open();

        int Close();

        int Decode(byte[] toDecode, out byte[] pcm);
    }
}