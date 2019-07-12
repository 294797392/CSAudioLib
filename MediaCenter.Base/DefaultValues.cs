using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Kagura.Player.Base
{
    public static class DefaultValues
    {
        public static readonly string MPlayerExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mplayer.exe");
        public const int ProgressTimerInterval = 1000;

        public const string GMusicFileStreamName = "GMUSIC_FILE_STREAM";

        public static int GMusicStreamMaximumReadSize = 64 * 1024;
    }
}
