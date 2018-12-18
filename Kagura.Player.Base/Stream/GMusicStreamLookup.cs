using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMusicCore
{
    public class GMusicStreamLookup
    {
        private Dictionary<string, IGMusicStream> streamMap = new Dictionary<string, IGMusicStream>();

        public IGMusicStream Lookup(string musicUri)
        {
            var allStream = streamMap.Values;
            foreach (IGMusicStream stream in allStream)
            {
                if (stream.IsProtocolSupported(musicUri))
                {
                    return stream;
                }
            }

            // 没找到支持的音频流，默认使用本地文件音频流
            return this.streamMap[DefaultValues.GMusicFileStreamName];
        }
    }
}