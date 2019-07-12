using MediaCenter.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kagura.Player.Base
{
    public class StreamLookup
    {
        private Dictionary<string, AbstractAVStream> streamMap = new Dictionary<string, AbstractAVStream>();

        public AbstractAVStream Lookup(string uri)
        {
            var allStream = streamMap.Values;
            foreach (AbstractAVStream stream in allStream)
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