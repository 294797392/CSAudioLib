using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GMusicCore.Demuxer
{
    public class GMusicDemuxerLookup
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("GMusicDemuxerLookup");

        private Dictionary<GMusicFormats, IGMusicDemuxer> demuxerMap = new Dictionary<GMusicFormats, IGMusicDemuxer>();
        private Dictionary<string, IGMusicDemuxer> extensionMap = new Dictionary<string, IGMusicDemuxer>();

        public IGMusicDemuxer Lookup(IGMusicStream stream)
        {
            /*
             * 1.根据文件后缀名查找Demuxer
             * 2.挨个循环Demuxer，查找第一个可用的Demuxer
             */

            string extension;
            string filePath = stream.MusicSource.Uri;
            if (Path.HasExtension(filePath) && (extension = Path.GetExtension(filePath)) != null)
            {
                IGMusicDemuxer demuxer;
                if (extensionMap.TryGetValue(extension, out demuxer))
                {
                    logger.InfoFormat("demuxer found by extension, {0}", demuxer.Name);
                    return demuxer;
                }
            }

            var allDemuxer = this.demuxerMap.Values;
            foreach (IGMusicDemuxer demuxer in allDemuxer)
            {
                if (demuxer.Check())
                {
                    logger.InfoFormat("demuxer found by lookup, {0}", demuxer.Name);
                    return demuxer;
                }
            }

            logger.ErrorFormat("demuxer not found, uri:{0}", filePath);
            return null;
        }
    }
}