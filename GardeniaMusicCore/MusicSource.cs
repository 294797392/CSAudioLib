using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMusicCore
{
    public class MusicSource
    {
        public string ID { get; private set; }

        public string Uri { get; set; }

        public MusicSource()
        {
            this.ID = Guid.NewGuid().ToString();
        }
    }
}