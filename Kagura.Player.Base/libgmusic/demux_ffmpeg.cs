using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GMusicCore.libgmusic
{
    public static class demux_ffmpeg
    {
        private const string DLLNAME = "libgmusic.dll";

        public delegate int read(IntPtr dest, int size);

        [StructLayout(LayoutKind.Sequential)]
        public struct demux_ffmpeg_info
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
            public string uri;

            public read read;
        }

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "demux_ffmpeg_init")]
        public static extern IntPtr demux_ffmpeg_init();

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "demux_ffmpeg_check")]
        public static extern int demux_ffmpeg_check(IntPtr handle, demux_ffmpeg_info info);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "demux_ffmpeg_open")]
        public static extern int demux_ffmpeg_open(IntPtr handle, demux_ffmpeg_info info);
    }
}
