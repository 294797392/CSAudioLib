using GMusicCore.libgmusic;
using GMusicDemuxer;
using GMusicStream;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using static GMusicCore.libgmusic.demux_ffmpeg;

namespace DirectSoundLibTest
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static FileStream fs;
        private static FileStream fs1;

        public App()
        {
            KMFileStream fs = new KMFileStream(new GMusicCore.MusicSource() { Uri = "1.mp3" });
            fs.Open();
            GMusicAudioDemuxer demuxer = new GMusicAudioDemuxer();
            demuxer.Open(fs);
        }
    }
}
