using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMusicCore
{
    public class MP3Decoder : IAudioDecoder
    {
        public int Initialize()
        {
            return ResponseCode.Success;
        }

        public int Release()
        {
            return ResponseCode.Success;
        }

        public int Decode(byte[] toDecode, out byte[] pcm)
        {
            pcm = null;
            return ResponseCode.Success;
        }
    }
}