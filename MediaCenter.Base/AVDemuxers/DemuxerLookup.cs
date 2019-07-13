using MediaCenter.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MediaCenter.Base.AVDemuxers
{
    public class DemuxerLookup
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("DemuxerLookup");

        public AbstractAVDemuxer Lookup(AbstractAVStream stream)
        {
            string demuxer_name;
            AbstractAVDemuxer demuxer;

            // 1.根据文件后缀名查找Demuxer
            //string extension;
            //string filePath = stream.MusicSource.Uri;
            //if (Path.HasExtension(filePath) && (extension = Path.GetExtension(filePath)) != null)
            //{
            //    if (extensionMap.TryGetValue(extension, out demuxer_name))
            //    {
            //        logger.InfoFormat("demuxer found by extension, extension:{0}, demuxer:{1}", extension, demuxer_name);
            //        if (!DemuxerRegistry.FindDemuxer(demuxer_name, out demuxer))
            //        {
            //            return null;
            //        }
            //        return demuxer;
            //    }
            //}

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

            //logger.ErrorFormat("demuxer not found, uri:{0}", filePath);

            return null;
        }
    }
}