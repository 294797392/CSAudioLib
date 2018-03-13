using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// DirectSound函数用到的常量定义
/// </summary>
namespace DirectSoundLib
{
    public class DSSCL
    {
        /// <summary>
        /// Sets the normal level. This level has the smoothest multitasking and resource-sharing behavior, but because it does not allow the primary buffer format to change, output is restricted to the default 8-bit format. 
        /// </summary>
        public static readonly uint DSSCL_NORMAL = 0x00000001;

        /// <summary>
        /// Sets the priority level. Applications with this cooperative level can call the SetFormat and Compact methods. 
        /// </summary>
        public static readonly uint DSSCL_PRIORITY = 0x00000002;

        /// <summary>
        /// For DirectX 8.0 and later, has the same effect as DSSCL_PRIORITY. For previous versions, sets the application to the exclusive level. This means that when it has the input focus, the application will be the only one audible; sounds from applications with the DSBCAPS_GLOBALFOCUS flag set will be muted. With this level, it also has all the privileges of the DSSCL_PRIORITY level. DirectSound will restore the hardware format, as specified by the most recent call to the SetFormat method, after the application gains the input focus. 
        /// </summary>
        public static readonly uint DSSCL_EXCLUSIVE = 0x00000003;

        /// <summary>
        /// Sets the write-primary level. The application has write access to the primary buffer. No secondary buffers can be played. This level cannot be set if the DirectSound driver is being emulated for the device; that is, if the GetCaps method returns the DSCAPS_EMULDRIVER flag in the DSCAPS structure. 
        /// </summary>
        public static readonly uint DSSCL_WRITEPRIMARY = 0x00000004;
    }

    public class DSBCAPS
    {
        public static readonly uint DSBCAPS_PRIMARYBUFFER = 0x00000001;

        public static readonly uint DSBCAPS_STATIC = 0x00000002;

        /// <summary>
        /// 缓冲区存储在声卡里, 混音是在声卡里做的
        /// </summary>
        public static readonly uint DSBCAPS_LOCHARDWARE = 0x00000004;

        /// <summary>
        /// 缓冲区存储在内存里, 混音是CPU做的
        /// </summary>
        public static readonly uint DSBCAPS_LOCSOFTWARE = 0x00000008;

        /// <summary>
        /// The sound source can be moved in 3D space. 
        /// </summary>
        public static readonly uint DSBCAPS_CTRL3D = 0x00000010;

        /// <summary>
        /// 可以控制声音的频率
        /// </summary>
        public static readonly uint DSBCAPS_CTRLFREQUENCY = 0x00000020;

        /// <summary>
        /// The sound source can be moved from left to right. 
        /// </summary>
        public static readonly uint DSBCAPS_CTRLPAN = 0x00000040;

        /// <summary>
        /// 可获取或设置音量大小
        /// </summary>
        public static readonly uint DSBCAPS_CTRLVOLUME = 0x00000080;

        /// <summary>
        /// 缓冲区通知功能
        /// </summary>
        public static readonly uint DSBCAPS_CTRLPOSITIONNOTIFY = 0x00000100;

        /// <summary>
        /// Effects can be added to the buffer. 
        /// </summary>
        public static readonly uint DSBCAPS_CTRLFX = 0x00000200;

        public static readonly uint DSBCAPS_STICKYFOCUS = 0x00004000;

        /// <summary>
        /// 失去焦点继续播放功能
        /// </summary>
        public static readonly uint DSBCAPS_GLOBALFOCUS = 0x00008000;

        public static readonly uint DSBCAPS_GETCURRENTPOSITION2 = 0x00010000;

        public static readonly uint DSBCAPS_MUTE3DATMAXDISTANCE = 0x00020000;

        public static readonly uint DSBCAPS_LOCDEFER = 0x00040000;

        public static readonly uint DSBCAPS_TRUEPLAYPOSITION = 0x00080000;
    }

    public class DSBPLAY
    {
        /// <summary>
        /// 缓冲区播放完毕之后从缓冲区开始的位置继续播放, 当播放主缓冲区的时候必须设置DSBPLAY_LOOPING
        /// </summary>
        public static readonly uint DSBPLAY_LOOPING = 0x00000001;

        public static readonly uint DSBPLAY_LOCHARDWARE = 0x00000002;

        public static readonly uint DSBPLAY_LOCSOFTWARE = 0x00000004;

        public static readonly uint DSBPLAY_TERMINATEBY_TIME = 0x00000008;

        public static readonly uint DSBPLAY_TERMINATEBY_DISTANCE = 0x000000010;

        public static readonly uint DSBPLAY_TERMINATEBY_PRIORITY = 0x000000020;
    }

    public class DSBSTATUS
    {
        public static readonly uint DSBSTATUS_PLAYING = 0x00000001;

        public static readonly uint DSBSTATUS_BUFFERLOST = 0x00000002;

        public static readonly uint DSBSTATUS_LOOPING = 0x00000004;

        public static readonly uint DSBSTATUS_LOCHARDWARE = 0x00000008;

        public static readonly uint DSBSTATUS_LOCSOFTWARE = 0x00000010;

        public static readonly uint DSBSTATUS_TERMINATED = 0x00000020;
    }

    public class DSBLOCK
    {
        public static readonly uint DSBLOCK_FROMWRITECURSOR = 0x00000001;

        public static readonly uint DSBLOCK_ENTIREBUFFER = 0x00000002;
    }

    public class DSBFREQUENCY
    {
        public static readonly uint DSBFREQUENCY_ORIGINAL = 0;
        public static readonly uint DSBFREQUENCY_MIN = 100;
#if DIRECTSOUND_VERSION_0x0900
        public static readonly uint DSBFREQUENCY_MAX = 200000;
#else
        public static readonly uint DSBFREQUENCY_MAX = 100000;
#endif
    }

    public class DS3DALG
    {
        public static readonly uint DS3DALG_DEFAULT = 0;
    }

    public class DSBVOLUME
    {
        public const int DSBVOLUME_MAX = 0;
        public const int DSBVOLUME_MIN = -10000;
    }
}