using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMusicCore.StreamReader
{
    public class StreamReaderFactory
    {
        public static IStreamReader Create(Uri uri)
        {
            IStreamReader reader = null;
            if (uri.Scheme.StartsWith("http"))
            {
                reader = new HttpStreamReader();
            }
            else
            {
                reader = new FileStreamReader();
            }
            reader.Source = uri;
            return reader;
        }
    }
}