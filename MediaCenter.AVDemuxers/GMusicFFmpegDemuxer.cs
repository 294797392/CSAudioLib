using GMusicCore;
using GMusicCore.Demuxer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMusicDemuxer
{
    public class GMusicFFmpegDemuxer : IGMusicDemuxer
    {
        public override string Name
        {
            get
            {
                return GMusicDemuxerNames.DEMUXER_FFMPEG;
            }
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override int Open(IGMusicStream stream)
        {
            throw new NotImplementedException();
        }
    }
}
