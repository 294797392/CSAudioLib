using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace libgmusic
{
    public enum mplayer_event_enum
    {
        MPEVT_STATUS_CHANGED
    }

    public enum mplayer_status_enum
    {
        MPSTAT_PLAYING,
        MPSTAT_PAUSED,
        MPSTAT_STOPPED
    }

    public struct mplayer_listener
    {
        public Func<mplayer_event_enum, IntPtr, IntPtr> mp_event_handler;
        public IntPtr userdata;
    }

    /// <summary>
    /// 对libgmusic mplayer的C#封装
    /// </summary>
    public static class mplayer
    {
        public const int MP_FAILED = -1;
        public const int MP_SUCCESS = 0;
        public const int MP_CREATE_PROCESS_FAILED = 1;
        public const int MP_SEND_COMMAND_FAILED = 2;
        public const int MP_READ_DATA_FAILED = 3;

        private const string LIBGMUSIC_DLL_NAME = "libgmusic.dll";

        [DllImport(LIBGMUSIC_DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mplayer_create_instance")]
        public static extern IntPtr mplayer_create_instance();

        [DllImport(LIBGMUSIC_DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mplayer_free_instance")]
        public static extern void mplayer_free_instance(IntPtr mphandle);

        [DllImport(LIBGMUSIC_DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mplayer_open")]
        public static extern void mplayer_open(IntPtr mphandle, IntPtr source, int size);

        [DllImport(LIBGMUSIC_DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mplayer_close")]
        public static extern void mplayer_close(IntPtr mphandle);

        [DllImport(LIBGMUSIC_DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mplayer_play")]
        public static extern int mplayer_play(IntPtr mphandle);

        [DllImport(LIBGMUSIC_DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mplayer_stop")]
        public static extern void mplayer_stop(IntPtr mphandle);

        [DllImport(LIBGMUSIC_DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mplayer_pause")]
        public static extern int mplayer_pause(IntPtr mphandle);

        [DllImport(LIBGMUSIC_DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mplayer_resume")]
        public static extern int mplayer_resume(IntPtr mphandle);

        [DllImport(LIBGMUSIC_DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mplayer_increase_volume")]
        public static extern void mplayer_increase_volume(IntPtr mphandle);

        [DllImport(LIBGMUSIC_DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mplayer_decrease_volume")]
        public static extern void mplayer_decrease_volume(IntPtr mphandle);

        [DllImport(LIBGMUSIC_DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mplayer_get_duration")]
        public static extern int mplayer_get_duration(IntPtr mphandle);

        [DllImport(LIBGMUSIC_DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mplayer_get_position")]
        public static extern int mplayer_get_position(IntPtr mphandle);

        [DllImport(LIBGMUSIC_DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mplayer_listen_event")]
        public static extern int mplayer_listen_event(IntPtr mphandle, mplayer_listener listener);

        [DllImport(LIBGMUSIC_DLL_NAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mplayer_get_status")]
        public static extern int mplayer_get_status(IntPtr mphandle);
    }
}