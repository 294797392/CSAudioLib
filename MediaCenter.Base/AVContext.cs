using MediaCenter.Base.AVDemuxers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaCenter.Base
{
    public class AVContext
    {
        public AbstractAVDemuxer AVDemuxer { get; set; }

        public AbstractAVStream AVStream { get; set; }
    }
}
