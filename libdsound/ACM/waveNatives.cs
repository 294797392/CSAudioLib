using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CSAudioLib.ACM
{
    public class BOOL
    {
        public const int TRUE = 1;
        public const int FALSE = 0;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct wavehdr_tag
    {
        /// <summary>
        /// Pointer to the waveform buffer.
        /// </summary>
        public IntPtr lpData;               /* pointer to locked data buffer */

        /// <summary>
        /// Length, in bytes, of the buffer.
        /// </summary>
        public uint dwBufferLength;         /* length of data buffer */

        /// <summary>
        /// When the header is used in input, specifies how much data is in the buffer.
        /// </summary>
        public uint dwBytesRecorded;        /* used for input only */

        /// <summary>
        /// User data.
        /// </summary>
        public uint dwUser;                 /* for client's use */
        public uint dwFlags;                /* assorted flags (see defines) */
        public uint dwLoops;                /* loop control counter */
        public IntPtr lpNext;               /* reserved for driver */
        public uint reserved;               /* reserved for driver */
    }

    public class waveNatives
    {
        #region 枚举

        public enum uMsgEnum : uint
        {
            /// <summary>
            /// Sent when the device is closed using the waveInClose function.
            /// </summary>
            WIM_CLOSE = 0x3BF,

            /// <summary>
            /// Sent when the device driver is finished with a data block sent using the waveInAddBuffer function.
            /// </summary>
            WIM_DATA = 0x3C0,

            /// <summary>
            /// Sent when the device is opened using the waveInOpen function.
            /// </summary>
            WIM_OPEN = 0x3BE
        }

        #endregion

        #region DeviceID

        public const uint WAVE_MAPPER = 0xFFFFFFFF;

        #endregion

        #region fdwOpen选项

        /// <summary>
        /// The dwCallback parameter is an event handle.
        /// </summary>
        public const int CALLBACK_EVENT = 0x00050000;

        /// <summary>
        /// The dwCallback parameter is a callback procedure address.
        /// </summary>
        public const int CALLBACK_FUNCTION = 0x00030000;

        /// <summary>
        /// No callback mechanism. This is the default setting.
        /// </summary>
        public const int CALLBACK_NULL = 0x00000000;

        /// <summary>
        /// The dwCallback parameter is a thread identifier.
        /// </summary>
        public const int CALLBACK_THREAD = 0x00020000;

        /// <summary>
        /// The dwCallback parameter is a window handle.
        /// </summary>
        public const int CALLBACK_WINDOW = 0x00010000;

        /// <summary>
        /// If this flag is specified, the ACM driver does not perform conversions on the audio data.
        /// </summary>
        public const int WAVE_FORMAT_DIRECT = 0x0008;

        /// <summary>
        /// The function queries the device to determine whether it supports the given format, but it does not open the device.
        /// </summary>
        public const int WAVE_FORMAT_QUERY = 0x0001;

        /// <summary>
        /// The uDeviceID parameter specifies a waveform-audio device to be mapped to by the wave mapper.
        /// </summary>
        public const int WAVE_MAPPED = 0x0004;

        #endregion

        #region waveIn函数

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwi"></param>
        /// <param name="uMsg"></param>
        /// <param name="dwInstance"></param>
        /// <param name="dwParam1">wavehdr_tag指针</param>
        /// <param name="dwParam2"></param>
        public delegate void waveInProcDlg(IntPtr hwi, uint uMsg, uint dwInstance, uint dwParam1, uint dwParam2);

        /// <summary>
        /// 获取音频输入设备的数量
        /// </summary>
        /// <returns></returns>
        [DllImport("Winmm", CallingConvention = CallingConvention.StdCall)]
        public static extern int waveInGetNumDevs();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phwi">用于返回设备句柄的指针; 如果 dwFlags=WAVE_FORMAT_QUERY, 这里应是 NULL</param>
        /// <param name="uDeviceID">设备ID; 可以指定为: WAVE_MAPPER, 这样函数会根据给定的波形格式选择合适的设备</param>
        /// <param name="pwfx">要申请的声音格式</param>
        /// <param name="dwCallback">回调函数地址或窗口句柄; 若不使用回调机制, 设为 NULL</param>
        /// <param name="dwInstance">给回调函数的实例数据; 不用于窗口</param>
        /// <param name="fdwOpen">打开选项</param>
        /// <returns></returns>
        [DllImport("Winmm", CallingConvention = CallingConvention.StdCall)]
        public static extern int waveInOpen([Out]out IntPtr phwi, [In]uint uDeviceID, [In]IntPtr pwfx, [In]waveInProcDlg dwCallback, [In]int dwInstance, [In]int fdwOpen);

        /// <summary>
        /// The waveInClose function closes the given waveform-audio input device.
        /// </summary>
        /// <param name="phwi"></param>
        /// <remarks>
        /// If there are input buffers that have been sent with the waveInAddBuffer function and that haven't been returned to the application, the close operation will fail. Call the waveInReset function to mark all pending buffers as done.
        /// 为了确保关闭成功, 要在调用waveInClose之前先调用waveInReset
        /// </remarks>
        /// <returns></returns>
        [DllImport("Winmm", CallingConvention = CallingConvention.StdCall)]
        public static extern int waveInClose([In]IntPtr phwi);

        [DllImport("Winmm", CallingConvention = CallingConvention.StdCall)]
        public static extern int waveInStart([In]IntPtr hwi);

        [DllImport("Winmm", CallingConvention = CallingConvention.StdCall)]
        public static extern int waveInStop([In]IntPtr hwi);

        [DllImport("Winmm", CallingConvention = CallingConvention.StdCall)]
        public static extern int waveInReset([In]IntPtr hwi);

        /// <summary>
        /// The waveInPrepareHeader function prepares a buffer for waveform-audio input.
        /// </summary>
        /// <param name="hwi">Handle to the waveform-audio input device.</param>
        /// <param name="pwh">WAVEHDR（wavehdr_tag）结构体指针</param>
        /// <param name="cbwh">Size, in bytes, of the WAVEHDR structure.</param>
        /// <remarks>
        /// The lpData, dwBufferLength, and dwFlags members of the WAVEHDR structure must be set before calling this function (dwFlags must be zero).
        /// </remarks>
        /// <returns></returns>
        [DllImport("Winmm", CallingConvention = CallingConvention.StdCall)]
        public static extern int waveInPrepareHeader([In]IntPtr hwi, [In]IntPtr pwh, uint cbwh);

        /// <summary>
        /// The waveInAddBuffer function sends an input buffer to the given waveform-audio input device. When the buffer is filled, the application is notified.
        /// 把准备好的缓冲区送给硬件
        /// </summary>
        /// <param name="hwi"></param>
        /// <param name="pwh">waveHeader指针</param>
        /// <param name="cbwh"></param>
        /// <remarks>
        /// When the buffer is filled, the WHDR_DONE bit is set in the dwFlags member of the WAVEHDR structure.
        /// The buffer must be prepared with the waveInPrepareHeader function before it is passed to this function.
        /// 在调用waveInAddBuffer之前必须调用waveInPrepareHeader函数
        /// </remarks>
        /// <returns></returns>
        [DllImport("Winmm", CallingConvention = CallingConvention.StdCall)]
        public static extern int waveInAddBuffer([In]IntPtr hwi, [In]IntPtr pwh, uint cbwh);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwi"></param>
        /// <param name="pwh">waveHeader指针</param>
        /// <param name="cbwh"></param>
        /// <returns></returns>
        [DllImport("Winmm", CallingConvention = CallingConvention.StdCall)]
        public static extern int waveInUnprepareHeader([In]IntPtr hwi, [In]IntPtr pwh, uint cbwh);

        #endregion
    }
}