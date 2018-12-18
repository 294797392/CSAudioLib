using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GMusicCore.libgmusic
{
    public static class helper
    {
        private const string DLLNAME = "libgmusic.dll";

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "helper_mp3_frame_head")]
        public static extern UInt32 helper_mp3_frame_head(byte b1, byte b2, byte b3, byte b4);
    }
}
