using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// ACM函数用到的常量定义
/// </summary>
namespace CSAudioLib.ACM
{
    public class CALLBACK
    {
        /// <summary>
        /// The dwCallback parameter is a handle of an event.
        /// </summary>
        public static readonly int CALLBACK_EVENT = 0x00050000;

        /// <summary>
        /// The dwCallback parameter is a callback procedure address. The function prototype must conform to the acmStreamConvertCallback prototype.
        /// </summary>
        public static readonly int CALLBACK_FUNCTION = 0x00030000;

        /// <summary>
        /// The dwCallback parameter is a window handle.
        /// </summary>
        public static readonly int CALLBACK_WINDOW = 0x00010000;
    }

    public class ACM_STREAMSIZEF
    {
        /// <summary>
        /// The cbInput parameter contains the size of the source buffer. The pdwOutputBytes parameter will receive the recommended destination buffer size, in bytes.
        /// </summary>
        public static readonly uint ACM_STREAMSIZEF_SOURCE = 0x00000000;

        /// <summary>
        /// The cbInput parameter contains the size of the destination buffer. The pdwOutputBytes parameter will receive the recommended source buffer size, in bytes.
        /// </summary>
        public static readonly uint ACM_STREAMSIZEF_DESTINATION = 0x00000001;


        public static readonly uint ACM_STREAMSIZEF_QUERYMASK = 0x0000000F;
    }

    public class ACM_STREAMOPENF
    {
        /// <summary>
        /// ACM will be queried to determine whether it supports the given conversion. A conversion stream will not be opened, and no handle will be returned in the phas parameter.
        /// </summary>
        public static readonly int ACM_STREAMOPENF_QUERY = 0x00000001;

        /// <summary>
        /// Stream conversion should be performed asynchronously. If this flag is specified, the application can use a callback function to be notified when the conversion stream is opened and closed and after each buffer is converted. In addition to using a callback function, an application can examine the fdwStatus member of the ACMSTREAMHEADER structure for the ACMSTREAMHEADER_STATUSF_DONE flag.
        /// </summary>
        public static readonly int ACM_STREAMOPENF_ASYNC = 0x00000002;

        /// <summary>
        /// ACM will not consider time constraints when converting the data. By default, the driver will attempt to convert the data in real time. For some formats, specifying this flag might improve the audio quality or other characteristics.
        /// </summary>
        public static readonly int ACM_STREAMOPENF_NONREALTIME = 0x00000004;
    }

    public class ACM_STREAMCONVERTF
    {
        /// <summary>
        /// Only integral numbers of blocks will be converted. Converted data will end on block-aligned boundaries. An application should use this flag for all conversions on a stream until there is not enough source data to convert to a block-aligned destination. In this case, the last conversion should be specified without this flag.
        /// </summary>
        public static readonly uint ACM_STREAMCONVERTF_BLOCKALIGN = 0x00000004;

        /// <summary>
        /// ACM conversion stream should begin returning pending instance data. For example, if a conversion stream holds instance data, such as the end of an echo filter operation, this flag will cause the stream to start returning this remaining data with optional source data. This flag can be specified with the ACM_STREAMCONVERTF_START flag.
        /// </summary>
        public static readonly uint ACM_STREAMCONVERTF_END = 0x00000020;

        /// <summary>
        /// ACM conversion stream should reinitialize its instance data. For example, if a conversion stream holds instance data, such as delta or predictor information, this flag will restore the stream to starting defaults. This flag can be specified with the ACM_STREAMCONVERTF_END flag.
        /// </summary>
        public static readonly int ACM_STREAMCONVERTF_START = 0x00000010;
    }

    public class ACM_METRIC
    {
        /// <summary>
        /// Returned value is the total number of enabled global ACM drivers (of all support types) in the system. The hao parameter must be NULL for this metric index. The pMetric parameter must point to a buffer of a size equal to a DWORD value.
        /// </summary>
        public static readonly uint ACM_METRIC_COUNT_DRIVERS = 1;

        /// <summary>
        /// Returned value is the number of global ACM compressor or decompressor drivers in the system. The hao parameter must be NULL for this metric index. The pMetric parameter must point to a buffer of a size equal to a DWORD value.
        /// </summary>
        public static readonly uint ACM_METRIC_COUNT_CODECS = 2;

        /// <summary>
        /// Returned value is the number of global ACM converter drivers in the system. The hao parameter must be NULL for this metric index. The pMetric parameter must point to a buffer of a size equal to a DWORD value.
        /// </summary>
        public static readonly uint ACM_METRIC_COUNT_CONVERTERS = 3;

        /// <summary>
        /// Returned value is the number of global ACM filter drivers in the system. The hao parameter must be NULL for this metric index. The pMetric parameter must point to a buffer of a size equal to a DWORD value.
        /// </summary>
        public static readonly uint ACM_METRIC_COUNT_FILTERS = 4;

        /// <summary>
        /// Returned value is the total number of global disabled ACM drivers (of all support types) in the system. The hao parameter must be NULL for this metric index. The pMetric parameter must point to a buffer of a size equal to a DWORD value. The sum of the ACM_METRIC_COUNT_DRIVERS and ACM_METRIC_COUNT_DISABLED metric indices is the total number of globally installed ACM drivers.
        /// </summary>
        public static readonly uint ACM_METRIC_COUNT_DISABLED = 5;

        /// <summary>
        /// Returned value is the number of global ACM hardware drivers in the system. The hao parameter must be NULL for this metric index. The pMetric parameter must point to a buffer of a size equal to a DWORD value.
        /// </summary>
        public static readonly uint ACM_METRIC_COUNT_HARDWARE = 6;

        /// <summary>
        /// Returned value is the total number of enabled local ACM drivers (of all support types) for the calling task. The hao parameter must be NULL for this metric index. The pMetric parameter must point to a buffer of a size equal to a DWORD value.
        /// </summary>
        public static readonly uint ACM_METRIC_COUNT_LOCAL_DRIVERS = 20;

        /// <summary>
        /// Returned value is the number of local ACM compressor drivers, ACM decompressor drivers, or both for the calling task. The hao parameter must be NULL for this metric index. The pMetric parameter must point to a buffer of a size equal to a DWORD value.
        /// </summary>
        public static readonly uint ACM_METRIC_COUNT_LOCAL_CODECS = 21;

        /// <summary>
        /// Returned value is the number of local ACM converter drivers for the calling task. The hao parameter must be NULL for this metric index. The pMetric parameter must point to a buffer of a size equal to a DWORD value.
        /// </summary>
        public static readonly uint ACM_METRIC_COUNT_LOCAL_CONVERTERS = 22;

        /// <summary>
        /// Returned value is the number of local ACM filter drivers for the calling task. The hao parameter must be NULL for this metric index. The pMetric parameter must point to a buffer of a size equal to a DWORD value.
        /// </summary>
        public static readonly uint ACM_METRIC_COUNT_LOCAL_FILTERS = 23;

        /// <summary>
        /// Returned value is the total number of local disabled ACM drivers, of all support types, for the calling task. The hao parameter must be NULL for this metric index. The pMetric parameter must point to a buffer of a size equal to a DWORD value. The sum of the ACM_METRIC_COUNT_LOCAL_DRIVERS and ACM_METRIC_COUNT_LOCAL_DISABLED metric indices is the total number of locally installed ACM drivers.
        /// </summary>
        public static readonly uint ACM_METRIC_COUNT_LOCAL_DISABLED = 24;

        /// <summary>
        /// Returned value is the waveform-audio input device identifier associated with the specified driver. The hao parameter must be a valid ACM driver identifier of the HACMDRIVERID data type that supports the ACMDRIVERDETAILS_SUPPORTF_HARDWARE flag. If no waveform-audio input device is associated with the driver, MMSYSERR_NOTSUPPORTED is returned. The pMetric parameter must point to a buffer of a size equal to a DWORD value.
        /// </summary>
        public static readonly uint ACM_METRIC_HARDWARE_WAVE_INPUT = 30;

        /// <summary>
        /// Returned value is the waveform-audio output device identifier associated with the specified driver. The hao parameter must be a valid ACM driver identifier of the HACMDRIVERID data type that supports the ACMDRIVERDETAILS_SUPPORTF_HARDWARE flag. If no waveform-audio output device is associated with the driver, MMSYSERR_NOTSUPPORTED is returned. The pMetric parameter must point to a buffer of a size equal to a DWORD value.
        /// </summary>
        public static readonly uint ACM_METRIC_HARDWARE_WAVE_OUTPUT = 31;

        /// <summary>
        /// Returned value is the size of the largest WAVEFORMATEX structure. If hao is NULL, the return value is the largest WAVEFORMATEX structure in the system. If hao identifies an open instance of an ACM driver of the HACMDRIVER data type or an ACM driver identifier of the HACMDRIVERID data type, the largest WAVEFORMATEX structure for that driver is returned. The pMetric parameter must point to a buffer of a size equal to a DWORD value. This metric is not allowed for an ACM stream handle of the HACMSTREAM data type.
        /// </summary>
        public static readonly uint ACM_METRIC_MAX_SIZE_FORMAT = 50;

        /// <summary>
        /// Returned value is the size of the largest WAVEFILTER structure. If hao is NULL, the return value is the largest WAVEFILTER structure in the system. If hao identifies an open instance of an ACM driver of the HACMDRIVER data type or an ACM driver identifier of the HACMDRIVERID data type, the largest WAVEFILTER structure for that driver is returned. The pMetric parameter must point to a buffer of a size equal to a DWORD value. This metric is not allowed for an ACM stream handle of the HACMSTREAM data type.
        /// </summary>
        public static readonly uint ACM_METRIC_MAX_SIZE_FILTER = 51;

        /// <summary>
        /// Returned value is the fdwSupport flags for the specified driver. The hao parameter must be a valid ACM driver identifier of the HACMDRIVERID data type. The pMetric parameter must point to a buffer of a size equal to a DWORD value.
        /// </summary>
        public static readonly uint ACM_METRIC_DRIVER_SUPPORT = 100;

        /// <summary>
        /// Returned value is the current priority for the specified driver. The hao parameter must be a valid ACM driver identifier of the HACMDRIVERID data type. The pMetric parameter must point to a buffer of a size equal to a DWORD value.
        /// </summary>
        public static readonly uint ACM_METRIC_DRIVER_PRIORITY = 101;
    }

    public class ACM_FORMATSUGGESTF
    {
        /// <summary>
        /// The wFormatTag member of the structure pointed to by pwfxDst is valid. The ACM will query acceptable installed drivers that can suggest a destination format matching wFormatTag or fail.
        /// </summary>
        public static readonly uint ACM_FORMATSUGGESTF_WFORMATTAG = 0x00010000;

        /// <summary>
        /// The nChannels member of the structure pointed to by pwfxDst is valid. The ACM will query acceptable installed drivers that can suggest a destination format matching nChannels or fail.
        /// </summary>
        public static readonly uint ACM_FORMATSUGGESTF_NCHANNELS = 0x00020000;

        /// <summary>
        /// The nSamplesPerSec member of the structure pointed to by pwfxDst is valid. The ACM will query acceptable installed drivers that can suggest a destination format matching nSamplesPerSec or fail.
        /// </summary>
        public static readonly uint ACM_FORMATSUGGESTF_NSAMPLESPERSEC = 0x00040000;

        /// <summary>
        /// The wBitsPerSample member of the structure pointed to by pwfxDst is valid. The ACM will query acceptable installed drivers that can suggest a destination format matching wBitsPerSample or fail.
        /// </summary>
        public static readonly uint ACM_FORMATSUGGESTF_WBITSPERSAMPLE = 0x00080000;

        public static readonly uint ACM_FORMATSUGGESTF_TYPEMASK = 0x00FF0000;
    }

    public class WAVE_FORMAT
    {
        public static readonly ushort WAVE_FORMAT_PCM = 1;

        //public static readonly ushort WAVE_FORMAT_MPEGLAYER3 = 0x0055;

        public static readonly ushort WAVE_FORMAT_GSM610 = 0x0031;

        public static readonly ushort WAVE_FORMAT_ALAW = 0x0006;

        public static readonly ushort WAVE_FORMAT_MULAW = 0x0007;
    }
}