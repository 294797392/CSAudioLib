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

        private Dictionary<string, string> extensionMap = new Dictionary<string, string>()
        {
            { "mp3",  GMusicDemuxerNames.DEMUXER_AUDIO },
            { "wav",  GMusicDemuxerNames.DEMUXER_AUDIO },
            { "flac",  GMusicDemuxerNames.DEMUXER_AUDIO },
            { "fla",  GMusicDemuxerNames.DEMUXER_AUDIO },
        };

        public IGMusicDemuxer Lookup(IGMusicStream stream)
        {
            string demuxer_name;
            IGMusicDemuxer demuxer;

            // 1.根据文件后缀名查找Demuxer
            string extension;
            string filePath = stream.MusicSource.Uri;
            if (Path.HasExtension(filePath) && (extension = Path.GetExtension(filePath)) != null)
            {
                if (extensionMap.TryGetValue(extension, out demuxer_name))
                {
                    logger.InfoFormat("demuxer found by extension, extension:{0}, demuxer:{1}", extension, demuxer_name);
                    if (!DemuxerRegistry.FindDemuxer(demuxer_name, out demuxer))
                    {
                        return null;
                    }
                    return demuxer;
                }
            }

            //// 2.挨个循环Demuxer，查找第一个可用的Demuxer
            //var allDemuxer = this.demuxerMap.Values;
            //foreach (IGMusicDemuxer demuxer in allDemuxer)
            //{
            //    if (demuxer.Check())
            //    {
            //        logger.InfoFormat("demuxer found by lookup, {0}", demuxer.Name);
            //        return demuxer;
            //    }
            //}

            logger.ErrorFormat("demuxer not found, uri:{0}", filePath);

            return null;
        }

        private void OpenDemuxer(IGMusicDemuxer demuxer)
        {

        }
    }
}