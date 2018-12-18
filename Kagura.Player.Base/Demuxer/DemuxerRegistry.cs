using ICare.Utility.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMusicCore.Demuxer
{
    public static class DemuxerRegistry
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("DemuxerRegistry");

        /// <summary>
        /// DemuxerName -> Demuxer ClassEntry
        /// </summary>
        private static Dictionary<string, string> registry = new Dictionary<string, string>();

        /// <summary>
        /// DemuxerName -> Demuxer Instance
        /// </summary>
        private static Dictionary<string, IGMusicDemuxer> demuxerMap = new Dictionary<string, IGMusicDemuxer>();

        static DemuxerRegistry()
        {

        }

        public static bool FindDemuxer(string demuxer_name, out IGMusicDemuxer demuxer)
        {
            if (!demuxerMap.TryGetValue(demuxer_name, out demuxer))
            {
                string classEntry;
                if (!registry.TryGetValue(demuxer_name, out classEntry))
                {
                    logger.ErrorFormat("cannot find {0}", demuxer_name);
                    return false;
                }

                try
                {
                    demuxer = ConfigFactory<IGMusicDemuxer>.CreateInstance<IGMusicDemuxer>(classEntry);
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("创建{0}实例异常", ex);
                    return false;
                }

                demuxerMap[demuxer_name] = demuxer;
            }

            return true;
        }

        public static IEnumerable<IGMusicDemuxer> GetAllDemuxers()
        {
            throw new NotImplementedException();
        }
    }
}