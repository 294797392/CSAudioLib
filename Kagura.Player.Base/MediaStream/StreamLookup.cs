using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kagura.Player.Base
{
    public class StreamLookup
    {
        private Dictionary<string, IStream> streamMap = new Dictionary<string, IStream>();

        public IStream Lookup(string uri)
        {
            var allStream = streamMap.Values;
            foreach (IStream stream in allStream)
            {
                if (stream.IsSupported(uri))
                {
                    return stream;
                }
            }

            return null;
        }
    }
}