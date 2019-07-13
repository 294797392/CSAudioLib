using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CSAudioLib.ACM
{
    /// <summary>
    /// 驱动详细信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMDRIVERDETAILS
    {
        /// <summary>
        /// Size, in bytes, of the valid information contained in the ACMDRIVERDETAILS structure. An application should initialize this member to the size, in bytes, of the desired information. The size specified in this member must be large enough to contain the cbStruct member of the ACMDRIVERDETAILS structure. When the acmDriverDetails function returns, this member contains the actual size of the information returned. The returned information will never exceed the requested size.
        /// </summary>
        public int cbStruct;

        /// <summary>
        /// Type of the driver. For ACM drivers, set this member to ACMDRIVERDETAILS_FCCTYPE_AUDIOCODEC.
        /// </summary>
        public int fccType;

        /// <summary>
        /// Subtype of the driver. This member is currently set to ACMDRIVERDETAILS_FCCCOMP_UNDEFINED (zero).
        /// </summary>
        public int fccComp;

        /// <summary>
        /// 制造商识别码
        /// </summary>
        public short wMid;

        /// <summary>
        /// 产品标识
        /// </summary>
        public short wPid;

        /// <summary>
        /// Version of the ACM for which this driver was compiled. The version number is a hexadecimal number in the format 0xAABBCCCC, where AA is the major version number, BB is the minor version number, and CCCC is the build number. The version parts (major, minor, and build) should be displayed as decimal numbers.
        /// </summary>
        public int vdwACM;

        /// <summary>
        /// Version of the driver. The version number is a hexadecimal number in the format 0xAABBCCCC, where AA is the major version number, BB is the minor version number, and CCCC is the build number. The version parts (major, minor, and build) should be displayed as decimal numbers.
        /// </summary>
        public int vdwDriver;

        /// <summary>
        /// acmNatives.fdwSupport
        /// </summary>
        public int fdwSupport;

        /// <summary>
        /// Number of unique format tags supported by this driver.
        /// </summary>
        public int cFormatTags;

        /// <summary>
        /// Number of unique filter tags supported by this driver.
        /// </summary>
        public int cFilterTags;

        /// <summary>
        /// Handle to a custom icon for this driver. An application can use this icon for referencing the driver visually. This member can be NULL.
        /// </summary>
        public IntPtr hicon;

        /// <summary>
        /// Null-terminated string that describes the name of the driver. This string is intended to be displayed in small spaces.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] szShortName;

        /// <summary>
        /// Null-terminated string that describes the full name of the driver. This string is intended to be displayed in large (descriptive) spaces.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] szLongName;

        /// <summary>
        /// Null-terminated string that provides copyright information for the driver.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        public byte[] szCopyright;

        /// <summary>
        /// Null-terminated string that provides special licensing information for the driver.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] szLicensing;

        /// <summary>
        /// Null-terminated string that provides special feature information for the driver.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] szFeatures;
    }

    /// <summary>
    /// Filter详细信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMFILTERDETAILS
    {
        /// <summary>
        /// Size, in bytes, of the ACMFILTERDETAILS structure. This member must be initialized before calling the acmFilterDetails or acmFilterEnum functions. The size specified in this member must be large enough to contain the base ACMFILTERDETAILS structure. When the acmFilterDetails function returns, this member contains the actual size of the information returned. The returned information will never exceed the requested size.
        /// </summary>
        public int cbStruct;

        /// <summary>
        /// Index of the filter about which details will be retrieved. The index ranges from zero to one less than the number of standard filters supported by an ACM driver for a filter tag. The number of standard filters supported by a driver for a filter tag is contained in the cStandardFilters member of the ACMFILTERTAGDETAILS structure. The dwFilterIndex member is used only when querying standard filter details about a driver by index; otherwise, this member should be zero. Also, this member will be set to zero by the ACM when an application queries for details on a filter; in other words, this member is used only for input and is never returned by the ACM or an ACM driver.
        /// </summary>
        public int dwFilterIndex;

        /// <summary>
        /// Waveform-audio filter tag that the ACMFILTERDETAILS structure describes. 
        /// This member is used as an input for the ACM_FILTERDETAILSF_INDEX query flag. 
        /// For the ACM_FILTERDETAILSF_FORMAT query flag, this member must be initialized to the same filter tag as the pwfltr member specifies. 
        /// If the acmFilterDetails function is successful, this member is always returned. 
        /// This member should be set to WAVE_FILTER_UNKNOWN for all other query flags.
        /// </summary>
        public int dwFilterTag;

        /// <summary>
        /// Driver-support flags specific to the specified filter. These flags are identical to the fdwSupport flags of the ACMDRIVERDETAILS structure, but they are specific to the filter that is being queried. This member can be a combination of the following values and identifies which operations the driver supports for the filter tag:
        /// acmNatives.fdwSupport
        /// </summary>
        public int fdwSupport;

        /// <summary>
        /// Pointer to a WAVEFILTER structure that will receive the filter details. 
        /// This structure requires no initialization by the application unless the ACM_FILTERDETAILSF_FILTER flag is specified with the acmFilterDetails function. 
        /// In this case, the dwFilterTag member of the WAVEFILTER structure must be equal to the dwFilterTag member of the ACMFILTERDETAILS structure.
        /// 存储WAVEFILTER结构体信息的指针，除非在调用acmFilterEnum的时候指定了ACM_FILTERENUMF_DWFILTERTAG标志，否则应用程序不需要初始化这个指针
        /// </summary>
        public IntPtr pwfltr;

        /// <summary>
        /// Size, in bytes, available for pwfltr to receive the filter details. 
        /// The acmMetrics and acmFilterTagDetails functions can be used to determine the maximum size required for any filter available for the specified driver (or for all installed ACM drivers).
        /// </summary>
        public int cbwfltr;

        /// <summary>
        /// String that describes the filter for the dwFilterTag type. If the acmFilterDetails function is successful, this string is always returned.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] szFilter;
    }

    /// <summary>
    /// The WAVEFILTER structure defines a filter for waveform-audio data. 
    /// Only filter information common to all waveform-audio data filters is included in this structure. 
    /// For filters that require additional information, this structure is included as the first member in another structure along with the additional information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WAVEFILTER
    {
        /// <summary>
        /// Size, in bytes, of the WAVEFILTER structure. 
        /// The size specified in this member must be large enough to contain the base WAVEFILTER structure.
        /// </summary>
        public int dbStruct;

        /// <summary>
        /// Waveform-audio filter type. Filter tags are registered with Microsoft Corporation for various filter algorithms.
        /// 滤波器类型
        /// </summary>
        public int dwFilterTag;

        /// <summary>
        /// Flags for the dwFilterTag member. 
        /// The flags defined for this member are universal to all filters. 
        /// Currently, no flags are defined.
        /// </summary>
        public int fdwFilter;

        /// <summary>
        /// Reserved for system use; should not be examined or modified by an application.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public int[] dwReserved;
    }

    /// <summary>
    /// Filter Tag详细信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMFILTERTAGDETAILS
    {
        /// <summary>
        /// Size, in bytes, of the ACMFILTERTAGDETAILS structure. This member must be initialized before an application calls the acmFilterTagDetails or acmFilterTagEnum function. The size specified in this member must be large enough to contain the base ACMFILTERTAGDETAILS structure. When the acmFilterTagDetails function returns, this member contains the actual size of the information returned. The returned information will never exceed the requested size.
        /// </summary>
        public int cbStruct;

        /// <summary>
        /// Index of the filter tag to retrieve details for. The index ranges from zero to one less than the number of filter tags supported by an ACM driver. The number of filter tags supported by a driver is contained in the cFilterTags member of the ACMDRIVERDETAILS structure. The dwFilterTagIndex member is used only when querying filter tag details about a driver by index; otherwise, this member should be zero.
        /// </summary>
        public int dwFilterTagIndex;

        /// <summary>
        /// Waveform-audio filter tag that the ACMFILTERTAGDETAILS structure describes. This member is used as an input for the ACM_FILTERTAGDETAILSF_FILTERTAG and ACM_FILTERTAGDETAILSF_LARGESTSIZE query flags. This member is always returned if the acmFilterTagDetails function is successful. This member should be set to WAVE_FILTER_UNKNOWN for all other query flags.
        /// </summary>
        public int dwFilterTag;

        /// <summary>
        /// Largest total size, in bytes, of a waveform-audio filter of the dwFilterTag type. For example, this member will be 40 for WAVE_FILTER_ECHO and 36 for WAVE_FILTER_VOLUME.
        /// </summary>
        public int cbFilterSize;

        /// <summary>
        /// Driver-support flags specific to the filter tag. These flags are identical to the fdwSupport flags of the ACMDRIVERDETAILS structure. This member can be a combination of the following values and identifies which operations the driver supports with the filter tag:
        /// acmNatives.fdwSupport
        /// </summary>
        public int fdwSupport;

        /// <summary>
        /// Number of standard filters of the dwFilterTag type (that is, the combination of all filter characteristics). This value cannot specify all filters supported by the driver.
        /// </summary>
        public int cStandardFilters;

        /// <summary>
        /// String that describes the dwFilterTag type. This string is always returned if the acmFilterTagDetails function is successful.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] szFilterTag;
    }

    /// <summary>
    /// Format详细信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMFORMATDETAILS
    {
        public int cbStruct;

        public int dwFormatIndex;

        public int dwFormatTage;

        /// <summary>
        /// 支持acmNatives.fdwSupport里的下列值:
        /// ACMDRIVERDETAILS_SUPPORTF_ASYNC
        /// ACMDRIVERDETAILS_SUPPORTF_CODEC
        /// ACMDRIVERDETAILS_SUPPORTF_CONVERTER
        /// ACMDRIVERDETAILS_SUPPORTF_FILTER
        /// ACMDRIVERDETAILS_SUPPORTF_HARDWARE
        /// </summary>
        public int fdwSupport;

        /// <summary>
        /// tWAVEFORMATTAG结构体指针
        /// Pointer to a WAVEFORMATEX structure that will receive the format details. This structure requires no initialization by the application unless the ACM_FORMATDETAILSF_FORMAT flag is specified in the acmFormatDetails function. In this case, the wFormatTag member of the WAVEFORMATEX structure must be equal to the dwFormatTag of the ACMFORMATDETAILS structure.
        /// </summary>
        public IntPtr pwfx;

        /// <summary>
        /// Size, in bytes, available for pwfx to receive the format details. The acmMetrics and acmFormatTagDetails functions can be used to determine the maximum size required for any format available for the specified driver (or for all installed ACM drivers).
        /// </summary>
        public int cbwfx;

        /// <summary>
        /// String that describes the format for the dwFormatTag type. If the acmFormatDetails function is successful, this string is always returned.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] szFormat;
    }

    /// <summary>
    /// StreamHeader
    /// Before an ACMSTREAMHEADER structure can be used for a conversion, it must be prepared by using the acmStreamPrepareHeader function. 
    /// When an application is finished with an ACMSTREAMHEADER structure, it must call the acmStreamUnprepareHeader function before freeing the source and destination buffers.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ACMSTREAMHEADER
    {
        /// <summary>
        /// Size, in bytes, of the ACMSTREAMHEADER structure. This member must be initialized before the application calls any ACM stream functions using this structure. 
        /// The size specified in this member must be large enough to contain the base ACMSTREAMHEADER structure.
        /// </summary>
        public int cbStruct;

        /// <summary>
        /// ACMSTREAMHEADER_STATUSF_DONE : Set by the ACM or driver to indicate that it is finished with the conversion and is returning the buffers to the application.
        /// ACMSTREAMHEADER_STATUSF_INQUEUE : Set by the ACM or driver to indicate that the buffers are queued for conversion.
        /// ACMSTREAMHEADER_STATUSF_PREPARED : Set by the ACM to indicate that the buffers have been prepared by using the acmStreamPrepareHeader function.
        /// </summary>
        public int fdwStatus;

        /// <summary>
        /// User data. This can be any instance data specified by the application.
        /// </summary>
        public int dwUser;

        /// <summary>
        /// Pointer to the source buffer. This pointer must always refer to the same location while the stream header remains prepared. If an application needs to change the source location, it must unprepare the header and reprepare it with the alternate location.
        /// </summary>
        public IntPtr pbSrc;

        /// <summary>
        /// Length, in bytes, of the source buffer pointed to by pbSrc. When the header is prepared, this member must specify the maximum size that will be used in the source buffer. Conversions can be performed on source lengths less than or equal to the original prepared size. However, this member must be reset to the original size when an application unprepares the header.
        /// </summary>
        public int cbSrcLength;

        /// <summary>
        /// Amount of data, in bytes, used for the conversion. This member is not valid until the conversion is complete. This value can be less than or equal to cbSrcLength. An application must use the cbSrcLengthUsed member when advancing to the next piece of source data for the conversion stream.
        /// </summary>
        public int cbSrcLengthUsed;

        /// <summary>
        /// User data. This can be any instance data specified by the application.
        /// </summary>
        public int dwSrcUser;

        /// <summary>
        /// Pointer to the destination buffer. This pointer must always refer to the same location while the stream header remains prepared. If an application needs to change the destination location, it must unprepare the header and reprepare it with the alternate location.
        /// </summary>
        public IntPtr pbDst;

        /// <summary>
        /// Length, in bytes, of the destination buffer pointed to by pbDst. When the header is prepared, this member must specify the maximum size that will be used in the destination buffer.
        /// </summary>
        public int cbDstLength;

        /// <summary>
        /// Amount of data, in bytes, returned by a conversion. This member is not valid until the conversion is complete. This value can be less than or equal to cbDstLength. An application must use the cbDstLengthUsed member when advancing to the next destination location for the conversion stream.
        /// </summary>
        public int cbDstLengthUsed;

        /// <summary>
        /// User data. This can be any instance data specified by the application.
        /// </summary>
        public int dwDstUser;

        /// <summary>
        /// Reserved; do not use. This member requires no initialization by the application and should never be modified while the header remains prepared.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] dwReservedDriver;
    }

    public class acmNatives
    {
        #region 常量

        /// <summary>
        /// A WAVEFILTER structure pointed to by the pwfltr member of the ACMFILTERDETAILS structure was given and the remaining details should be returned. The dwFilterTag member of the ACMFILTERDETAILS structure must be initialized to the same filter tag pwfltr specifies. This query type can be used to get a string description of an arbitrary filter structure. If an application specifies an ACM driver handle for had, details on the filter will be returned for that driver. If an application specifies NULL for had, the ACM finds the first acceptable driver to return the details.
        /// </summary>
        public const int ACM_FILTERDETAILSF_FILTER = 0x00000001;
        /// <summary>
        /// A filter index for the filter tag was given in the dwFilterIndex member of the ACMFILTERDETAILS structure. The filter details will be returned in the structure defined by pafd. The index ranges from zero to one less than the cStandardFilters member returned in the ACMFILTERTAGDETAILS structure for a filter tag. An application must specify a driver handle for had when retrieving filter details with this flag. For information about what members should be initialized before calling this function, see the ACMFILTERDETAILS structure.
        /// </summary>
        public const int ACM_FILTERDETAILSF_INDEX = 0x00000000;
        public const int ACM_FILTERDETAILSF_QUERYMASK = 0x0000000F;


        /// <summary>
        /// The dwFilterTag member of the WAVEFILTER structure pointed to by the pwfltr member of the ACMFILTERDETAILS structure is valid. The enumerator will enumerate only a filter that conforms to this attribute. The dwFilterTag member of the ACMFILTERDETAILS structure must be equal to the dwFilterTag member of the WAVEFILTER structure.
        /// </summary>
        public const int ACM_FILTERENUMF_DWFILTERTAG = 0x00010000;

        #endregion

        #region 滤波器类型

        public const int WAVE_FILTER_UNKNOWN = 0x0000;
        public const int WAVE_FILTER_DEVELOPMENT = 0xFFFF;
        public const int WAVE_FILTER_ECHO = 0x0002;

        #endregion

        #region acmDriver函数

        public enum fdwSupport : long
        {
            /// <summary>
            /// this flag is set if the driver supports
            /// conversions from one format tag to another format tag. for example, if a
            /// converter compresses WAVE_FORMAT_PCM to WAVE_FORMAT_ADPCM, then this bit
            /// should be set.
            /// </summary>
            ACMDRIVERDETAILS_SUPPORTF_CODEC = 0x00000001L,

            /// <summary>
            /// this flags is set if the driver
            /// supports conversions on the same format tag. as an example, the PCM
            /// converter that is built into the ACM sets this bit (and only this bit)
            /// because it converts only PCM formats (bits, sample rate).
            /// </summary>
            ACMDRIVERDETAILS_SUPPORTF_CONVERTER = 0x00000002L,

            /// <summary>
            /// this flag is set if the driver supports
            /// transformations on a single format. for example, a converter that changed
            /// the 'volume' of PCM data would set this bit. 'echo' and 'reverb' are
            /// also filter types.
            /// </summary>
            ACMDRIVERDETAILS_SUPPORTF_FILTER = 0x00000004L,

            /// <summary>
            /// this flag is set if the driver supports
            /// hardware input and/or output through a waveform device.
            /// </summary>
            ACMDRIVERDETAILS_SUPPORTF_HARDWARE = 0x00000008L,

            /// <summary>
            /// this flag is set if the driver supports
            /// async conversions.
            /// </summary>
            ACMDRIVERDETAILS_SUPPORTF_ASYNC = 0x00000010L,

            /// <summary>
            /// this flag is set _by the ACM_ if a
            /// driver has been installed local to the current task. this flag is also
            /// set in the fdwSupport argument to the enumeration callback function
            /// for drivers.
            /// </summary>
            ACMDRIVERDETAILS_SUPPORTF_LOCAL = 0x40000000L,

            /// <summary>
            /// this flag is set _by the ACM_ if a
            /// driver has been disabled. this flag is also passed set in the fdwSupport
            /// argument to the enumeration callback function for drivers.
            /// </summary>
            ACMDRIVERDETAILS_SUPPORTF_DISABLED = 0x80000000L
        }

        public enum fdwEnum : ulong
        {
            /// <summary>
            /// Only global drivers should be included in the enumeration.
            /// </summary>
            ACM_DRIVERENUMF_NOLOCAL = 0x40000000L,

            /// <summary>
            /// Disabled ACM drivers should be included in the enumeration. 
            /// Drivers can be disabled by the user through the Control Panel or by an application using the acmDriverPriority function. 
            /// If a driver is disabled, the fdwSupport parameter to the callback function will have the ACMDRIVERDETAILS_SUPPORTF_DISABLED flag set.
            /// </summary>
            ACM_DRIVERENUMF_DISABLED = 0x80000000L
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hadid">Handle to an ACM driver identifier.</param>
        /// <param name="dwInstance">Application-defined value specified in acmDriverEnum.</param>
        /// <param name="fdwSupport">
        /// Driver-support flags specific to the driver specified by hadid. These flags are identical to the fdwSupport flags of the ACMDRIVERDETAILS structure. This parameter can be a combination of the following values.
        /// ACMDRIVERDETAILS_SUPPORTF_ASYNC
        /// ACMDRIVERDETAILS_SUPPORTF_CODEC	
        /// ACMDRIVERDETAILS_SUPPORTF_CONVERTER
        /// ACMDRIVERDETAILS_SUPPORTF_DISABLED
        /// ACMDRIVERDETAILS_SUPPORTF_FILTER
        /// </param>
        /// <remarks>
        /// The acmDriverEnum function will return MMSYSERR_NOERROR (zero) if no ACM drivers are installed. Moreover, the callback function will not be called.
        /// The following functions should not be called from within the callback function: acmDriverAdd, acmDriverRemove, and acmDriverPriority.
        /// </remarks>
        /// <returns>The callback function must return TRUE to continue enumeration or FALSE to stop enumeration.</returns>
        public delegate int acmDriverEnumCallback(IntPtr hadid, int dwInstance, int fdwSupport);

        /// <summary>
        /// The acmDriverEnum function enumerates the available ACM drivers, continuing until there are no more drivers or the callback function returns FALSE.
        /// </summary>
        /// <param name="fnCallback">Procedure instance address of the application-defined callback function.</param>
        /// <param name="dwInstance">A 64-bit (DWORD_PTR) or 32-bit (DWORD) application-defined value that is passed to the callback function along with ACM driver information.</param>
        /// <param name="fdwEnum">
        /// Flags for enumerating ACM drivers.
        /// ACM_DRIVERENUMF_DISABLED : Disabled ACM drivers should be included in the enumeration. Drivers can be disabled by the user through the Control Panel or by an application using the acmDriverPriority function. If a driver is disabled, the fdwSupport parameter to the callback function will have the ACMDRIVERDETAILS_SUPPORTF_DISABLED flag set.
        /// ACM_DRIVERENUMF_NOLOCAL : Only global drivers should be included in the enumeration.
        /// acmNatives.fdwEnum
        /// </param>
        /// <remarks>
        /// The acmDriverEnum function will return MMSYSERR_NOERROR (zero) if no ACM drivers are installed. Moreover, the callback function will not be called.
        /// </remarks>
        /// <returns>
        /// MMSYSERR_INVALFLAG : At least one flag is invalid.
        /// MMSYSERR_INVALPARAM : At least one parameter is invalid.
        /// </returns>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern int acmDriverEnum(acmDriverEnumCallback fnCallback, int dwInstance, int fdwEnum);

        /// <summary>
        /// The acmDriverDetails function queries a specified ACM driver to determine its capabilities.
        /// </summary>
        /// <param name="hadid">Handle to the driver identifier of an installed ACM driver. Disabled drivers can be queried for details.</param>
        /// <param name="padd">Pointer to an ACMDRIVERDETAILS structure that will receive the driver details. The cbStruct member must be initialized to the size, in bytes, of the structure.</param>
        /// <param name="fdwDetails">Reserved; must be zero.</param>
        /// <returns></returns>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern int acmDriverDetails([In]IntPtr hadid, [In, Out]ref ACMDRIVERDETAILS padd, [In]int fdwDetails);

        /// <summary>
        /// The acmDriverOpen function opens the specified ACM driver and returns a driver instance handle that can be used to communicate with the driver.
        /// </summary>
        /// <param name="phad">Pointer to a buffer that receives the new driver instance handle that can be used to communicate with the driver.</param>
        /// <param name="hadid">Handle to the driver identifier of an installed and enabled ACM driver.</param>
        /// <param name="fdwOpen">Reserved; must be zero.</param>
        /// <returns></returns>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern int acmDriverOpen(out IntPtr phad, IntPtr hadid, int fdwOpen);

        /// <summary>
        /// The acmDriverClose function closes a previously opened ACM driver instance. If the function is successful, the handle is invalidated.
        /// </summary>
        /// <param name="had">Handle to the open driver instance to be closed.</param>
        /// <param name="fdwClose">Reserved; must be zero.</param>
        /// <returns></returns>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern int acmDriverClose(IntPtr had, int fdwClose);

        #endregion

        #region acmFilter函数

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hadid">Driver ID</param>
        /// <param name="pafd">ACMFILTERDETAILS结构体指针</param>
        /// <param name="dwInstance"></param>
        /// <param name="fdwSupport"></param>
        /// <returns></returns>
        public delegate int acmFilterEnumCallback(IntPtr hadid, IntPtr pafd, int dwInstance, int fdwSupport);

        /// <summary>
        /// The acmFilterEnum function enumerates waveform-audio filters available for a given filter tag from an ACM driver. This function continues enumerating until there are no more suitable filters for the filter tag or the callback function returns FALSE.
        /// </summary>
        /// <param name="had">Handle to the ACM driver to query for waveform-audio filter details. If this parameter is NULL, the ACM uses the details from the first suitable ACM driver.</param>
        /// <param name="pafd">Pointer to the ACMFILTERDETAILS structure that contains the filter details when it is passed to the function specified by fnCallback. When your application calls acmFilterEnum, the cbStruct, pwfltr, and cbwfltr members of this structure must be initialized. The dwFilterTag member must also be initialized to either WAVE_FILTER_UNKNOWN or a valid filter tag.</param>
        /// <param name="fnCallback">Procedure-instance address of the application-defined callback function.</param>
        /// <param name="dwInstance">A 32-bit (DWORD), 64-bit (DWORD_PTR) application-defined value that is passed to the callback function along with ACM filter details.</param>
        /// <param name="fdwEnum">
        /// ACM_FILTERENUMF_DWFILTERTAG	: The dwFilterTag member of the WAVEFILTER structure pointed to by the pwfltr member of the ACMFILTERDETAILS structure is valid. The enumerator will enumerate only a filter that conforms to this attribute. The dwFilterTag member of the ACMFILTERDETAILS structure must be equal to the dwFilterTag member of the WAVEFILTER structure.
        /// </param>
        /// <remarks>
        /// The acmFilterEnum function will return MMSYSERR_NOERROR (zero) if no suitable ACM drivers are installed. Moreover, the callback function will not be called.
        /// The following functions should not be called from within the callback function: acmDriverAdd, acmDriverRemove, and acmDriverPriority.
        /// </remarks>
        /// <returns></returns>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern int acmFilterEnum(IntPtr had, [In,Out]ref ACMFILTERDETAILS pafd, acmFilterEnumCallback fnCallback, int dwInstance, int fdwEnum);

        /// <summary>
        /// The acmFilterDetails function queries the ACM for details about a filter with a specific waveform-audio filter tag.
        /// </summary>
        /// <param name="had">Handle to the ACM driver to query for waveform-audio filter details for a filter tag. If this parameter is NULL, the ACM uses the details from the first suitable ACM driver.</param>
        /// <param name="pafd">Pointer to the ACMFILTERDETAILS structure that is to receive the filter details for the given filter tag.</param>
        /// <param name="fdwDetails">
        /// Flags for getting the details. The following values are defined.
        /// ACM_FILTERDETAILSF_FILTER : A WAVEFILTER structure pointed to by the pwfltr member of the ACMFILTERDETAILS structure was given and the remaining details should be returned. The dwFilterTag member of the ACMFILTERDETAILS structure must be initialized to the same filter tag pwfltr specifies. This query type can be used to get a string description of an arbitrary filter structure. If an application specifies an ACM driver handle for had, details on the filter will be returned for that driver. If an application specifies NULL for had, the ACM finds the first acceptable driver to return the details.
        /// ACM_FILTERDETAILSF_INDEX : A filter index for the filter tag was given in the dwFilterIndex member of the ACMFILTERDETAILS structure. The filter details will be returned in the structure defined by pafd. The index ranges from zero to one less than the cStandardFilters member returned in the ACMFILTERTAGDETAILS structure for a filter tag. An application must specify a driver handle for had when retrieving filter details with this flag. For information about what members should be initialized before calling this function, see the ACMFILTERDETAILS structure.
        /// </param>
        /// <returns></returns>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern int acmFilterDetails(IntPtr had, IntPtr pafd, int fdwDetails);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hadid">Driver ID</param>
        /// <param name="paftd">ACMFILTERTAGDETAILS结构体指针</param>
        /// <param name="dwInstance"></param>
        /// <param name="fdwSupport"></param>
        /// <returns></returns>
        public delegate int acmFilterTagEnumCallback(IntPtr hadid, IntPtr paftd, int dwInstance, int fdwSupport);

        /// <summary>
        /// The acmFilterTagEnum function enumerates waveform-audio filter tags available from an ACM driver. This function continues enumerating until there are no more suitable filter tags or the callback function returns FALSE.
        /// </summary>
        /// <param name="had">Handle to the ACM driver to query for waveform-audio filter tag details. If this parameter is NULL, the ACM uses the details from the first suitable ACM driver.</param>
        /// <param name="paftd">Pointer to the ACMFILTERTAGDETAILS structure that contains the filter tag details when it is passed to the fnCallback function. When your application calls acmFilterTagEnum, the cbStruct member of this structure must be initialized.</param>
        /// <param name="fnCallback">Procedure instance address of the application-defined callback function.</param>
        /// <param name="dwInstance">A 64-bit (DWORD_PTR) or 32-bit (DWORD) application-defined value that is passed to the callback function along with ACM filter tag details.</param>
        /// <param name="fdwEnum">Reserved; must be zero.</param>
        /// <remarks>
        /// This function will return MMSYSERR_NOERROR (zero) if no suitable ACM drivers are installed. Moreover, the callback function will not be called.
        /// </remarks>
        /// <returns></returns>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern int acmFilterTagEnum(IntPtr had, [In, Out]ref ACMFILTERTAGDETAILS paftd, acmFilterTagEnumCallback fnCallback, int dwInstance, int fdwEnum);

        #endregion

        #region acmFormat函数

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hadid"></param>
        /// <param name="pafd"></param>
        /// <param name="dwInstance"></param>
        /// <param name="fdwSupport">
        /// acmNatives.fdwSupport中的以下值:
        /// ACMDRIVERDETAILS_SUPPORTF_ASYNC
        /// ACMDRIVERDETAILS_SUPPORTF_CODEC
        /// ACMDRIVERDETAILS_SUPPORTF_CONVERTER
        /// ACMDRIVERDETAILS_SUPPORTF_FILTER
        /// ACMDRIVERDETAILS_SUPPORTF_HARDWARE
        /// </param>
        /// <returns>
        /// The callback function must return TRUE to continue enumeration or FALSE to stop enumeration.
        /// </returns>
        public delegate int acmFormatEnumCallback(IntPtr hadid, IntPtr pafd, int dwInstance, int fdwSupport);

        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern int acmFormatEnum(IntPtr had, [In, Out]ref ACMFORMATDETAILS pafd, acmFormatEnumCallback fnCallback, int dwInstance, int fdwEnum);

        /// <summary>
        /// The acmFormatSuggest function queries the ACM or a specified ACM driver to suggest a destination format for the supplied source format. For example, an application can use this function to determine one or more valid PCM formats to which a compressed format can be decompressed.
        /// </summary>
        /// <param name="had">Handle to an open instance of a driver to query for a suggested destination format. If this parameter is NULL, the ACM attempts to find the best driver to suggest a destination format.</param>
        /// <param name="pwfxSrc">Pointer to a WAVEFORMATEX structure that identifies the source format for which a destination format will be suggested by the ACM or specified driver.</param>
        /// <param name="pwfxDst">Pointer to a WAVEFORMATEX structure that will receive the suggested destination format for the pwfxSrc format. Depending on the fdwSuggest parameter, some members of the structure pointed to by pwfxDst may require initialization.</param>
        /// <param name="cbwfxDst">Size, in bytes, available for the destination format. The acmMetrics and acmFormatTagDetails functions can be used to determine the maximum size required for any format available for the specified driver (or for all installed ACM drivers).</param>
        /// <param name="fdwSuggest">Flags for matching the desired destination format. The following values are defined.</param>
        /// <returns></returns>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern uint acmFormatSuggest(IntPtr had, ref WAVEFORMATEX pwfxSrc, [In, Out]ref WAVEFORMATEX pwfxDst, uint cbwfxDst, uint fdwSuggest);

        #endregion

        #region acmStream函数

        public class MM_ACM_MSG
        {
            /// <summary>
            /// ACM has successfully opened the conversion stream identified by has.
            /// </summary>
            public const uint MM_ACM_OPEN = 0x3d4;

            /// <summary>
            /// ACM has successfully closed the conversion stream identified by has. The handle specified by has is no longer valid after receiving this message.
            /// </summary>
            public const uint MM_ACM_CLOSE = 0x3d5;

            /// <summary>
            /// ACM has successfully converted the buffer identified by lParam1 (which is a pointer to the ACMSTREAMHEADER structure) for the stream handle identified by has.
            /// </summary>
            public const uint MM_ACM_DONE = 0x3d6;
        }

        /// <summary>
        /// The acmStreamConvertCallback function specifies an application-provided callback function to be used when the acmStreamOpen function specifies the CALLBACK_FUNCTION flag. The acmStreamConvertCallback name is a placeholder for an application-defined function name.
        /// </summary>
        /// <param name="has"></param>
        /// <param name="uMsg">
        /// ACM conversion stream message. The following values are defined.
        /// MM_ACM_CLOSE : ACM has successfully closed the conversion stream identified by has. The handle specified by has is no longer valid after receiving this message.
        /// MM_ACM_DONE : ACM has successfully converted the buffer identified by lParam1 (which is a pointer to the ACMSTREAMHEADER structure) for the stream handle identified by has.
        /// MM_ACM_OPEN : ACM has successfully opened the conversion stream identified by has.
        /// </param>
        /// <param name="dwInstance">User-instance data given as the dwInstance parameter of the acmStreamOpen function.</param>
        /// <param name="lParam1">Message parameter.</param>
        /// <param name="lParam2">Message parameter.</param>
        /// <remarks>
        /// The following functions should not be called from within the callback function: acmDriverAdd, acmDriverRemove, and acmDriverPriority.
        /// </remarks>
        public delegate void acmStreamConvertCallbackDlg(IntPtr has, uint uMsg, uint dwInstance, int lParam1, int lParam2);

        /// <summary>
        /// The acmStreamOpen function opens an ACM conversion stream. Conversion streams are used to convert data from one specified audio format to another.
        /// </summary>
        /// <param name="phas">Pointer to a handle that will receive the new stream handle that can be used to perform conversions. This handle is used to identify the stream in calls to other ACM stream conversion functions. If the ACM_STREAMOPENF_QUERY flag is specified, this parameter should be NULL.</param>
        /// <param name="had">Handle to an ACM driver. If this handle is specified, it identifies a specific driver to be used for a conversion stream. If this parameter is NULL, all suitable installed ACM drivers are queried until a match is found.</param>
        /// <param name="pwfxSrc">Pointer to a WAVEFORMATEX structure that identifies the desired source format for the conversion.</param>
        /// <param name="pwfxDst">Pointer to a WAVEFORMATEX structure that identifies the desired destination format for the conversion.</param>
        /// <param name="pwfltr">Pointer to a WAVEFILTER structure that identifies the desired filtering operation to perform on the conversion stream. If no filtering operation is desired, this parameter can be NULL. If a filter is specified, the source (pwfxSrc) and destination (pwfxDst) formats must be the same.</param>
        /// <param name="dwCallback">Pointer to a callback function, a handle of a window, or a handle of an event. A callback function will be called only if the conversion stream is opened with the ACM_STREAMOPENF_ASYNC flag. A callback function is notified when the conversion stream is opened or closed and after each buffer is converted. If the conversion stream is opened without the ACM_STREAMOPENF_ASYNC flag, this parameter should be set to zero.</param>
        /// <param name="dwInstance">User-instance data passed to the callback function specified by the dwCallback parameter. This parameter is not used with window and event callbacks. If the conversion stream is opened without the ACM_STREAMOPENF_ASYNC flag, this parameter should be set to zero.</param>
        /// <param name="fdwOpen">ACM_STREAMOPENF, CALLBACK</param>
        /// <returns></returns>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern uint acmStreamOpen(out IntPtr phas, IntPtr had, ref WAVEFORMATEX  pwfxSrc, ref WAVEFORMATEX  pwfxDst, IntPtr pwfltr, acmStreamConvertCallbackDlg dwCallback, int dwInstance, int fdwOpen);

        /// <summary>
        /// The acmStreamClose function closes an ACM conversion stream. If the function is successful, the handle is invalidated.
        /// </summary>
        /// <param name="has">Stream Handle</param>
        /// <param name="fdwClose"></param>
        /// <returns></returns>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern uint acmStreamClose(IntPtr has, int fdwClose);

        /// <summary>
        /// The acmStreamPrepareHeader function prepares an ACMSTREAMHEADER structure for an ACM stream conversion. This function must be called for every stream header before it can be used in a conversion stream. An application needs to prepare a stream header only once for the life of a given stream. The stream header can be reused as long as the sizes of the source and destination buffers do not exceed the sizes used when the stream header was originally prepared.
        /// </summary>
        /// <param name="has"></param>
        /// <param name="pash">Pointer to an ACMSTREAMHEADER structure that identifies the source and destination buffers to be prepared.</param>
        /// <param name="fdwPrepare">Reserved; must be zero.</param>
        /// <remarks>
        /// Preparing a stream header that has already been prepared has no effect, and the function returns zero. Nevertheless, you should ensure your application does not prepare a stream header multiple times.
        /// </remarks>
        /// <returns></returns>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern uint acmStreamPrepareHeader(IntPtr has, [In]ref ACMSTREAMHEADER pash, int fdwPrepare);

        /// <summary>
        /// The acmStreamUnprepareHeader function cleans up the preparation performed by the acmStreamPrepareHeader function for an ACM stream. This function must be called after the ACM is finished with the given buffers. An application must call this function before freeing the source and destination buffers.
        /// </summary>
        /// <param name="has"></param>
        /// <param name="pash">Pointer to an ACMSTREAMHEADER structure that identifies the source and destination buffers to be unprepared.</param>
        /// <param name="fdwUnprepare">Reserved; must be zero.</param>
        /// <remarks>
        /// Unpreparing a stream header that has already been unprepared is an error. An application must specify the source and destination buffer lengths (cbSrcLength and cbDstLength, respectively) that were used during a call to the corresponding acmStreamPrepareHeader. Failing to reset these member values will cause acmStreamUnprepareHeader to fail with an MMSYSERR_INVALPARAM error.
        /// The ACM can recover from some errors. The ACM will return a nonzero error, yet the stream header will be properly unprepared. To determine whether the stream header was actually unprepared, an application can examine the ACMSTREAMHEADER_STATUSF_PREPARED flag. If acmStreamUnprepareHeader returns success, the header will always be unprepared.
        /// </remarks>
        /// <returns></returns>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern uint acmStreamUnprepareHeader(IntPtr has, ref ACMSTREAMHEADER pash, int fdwUnprepare);

        /// <summary>
        /// The acmStreamReset function stops conversions for a given ACM stream. All pending buffers are marked as done and returned to the application.
        /// </summary>
        /// <param name="has"></param>
        /// <param name="fdwReset"></param>
        /// <remarks>
        /// Resetting an ACM conversion stream is necessary only for asynchronous conversion streams. Resetting a synchronous conversion stream will succeed, but no action will be taken.
        /// </remarks>
        /// <returns></returns>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern int acmStreamReset(IntPtr has, int fdwReset);

        /// <summary>
        /// The acmStreamSize function returns a recommended size for a source or destination buffer on an ACM stream.
        /// </summary>
        /// <param name="has"></param>
        /// <param name="cbInput">Size, in bytes, of the source or destination buffer. The fdwSize flags specify what the input parameter defines. This parameter must be nonzero.</param>
        /// <param name="pdwOutputBytes">Pointer to a variable that contains the size, in bytes, of the source or destination buffer. The fdwSize flags specify what the output parameter defines. If the acmStreamSize function succeeds, this location will always be filled with a nonzero value.</param>
        /// <param name="fdwSize">
        /// Flags for the stream size query. The following values are defined:
        /// ACM_STREAMSIZEF_DESTINATION : The cbInput parameter contains the size of the destination buffer. The pdwOutputBytes parameter will receive the recommended source buffer size, in bytes.
        /// ACM_STREAMSIZEF_SOURCE : The cbInput parameter contains the size of the source buffer. The pdwOutputBytes parameter will receive the recommended destination buffer size, in bytes.
        /// </param>
        /// <remarks>
        /// An application can use this function to determine suggested buffer sizes for either source or destination buffers. The buffer sizes returned might be only an estimation of the actual sizes required for conversion. Because actual conversion sizes cannot always be determined without performing the conversion, the sizes returned will usually be overestimated.
        /// In the event of an error, the location pointed to by pdwOutputBytes will receive zero. This assumes that the pointer specified by pdwOutputBytes is valid.
        /// </remarks>
        /// <returns></returns>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern int acmStreamSize(IntPtr has, int cbInput, out int pdwOutputBytes, int fdwSize);

        /// <summary>
        /// The acmStreamConvert function requests the ACM to perform a conversion on the specified conversion stream. A conversion may be synchronous or asynchronous, depending on how the stream was opened.
        /// </summary>
        /// <param name="has"></param>
        /// <param name="pash">Pointer to a stream header that describes source and destination buffers for a conversion. This header must have been prepared previously by using the acmStreamPrepareHeader function.</param>
        /// <param name="fdwConvert">
        /// Flags for doing the conversion. The following values are defined.
        /// ACM_STREAMCONVERTF_BLOCKALIGN : Only integral numbers of blocks will be converted. Converted data will end on block-aligned boundaries. An application should use this flag for all conversions on a stream until there is not enough source data to convert to a block-aligned destination. In this case, the last conversion should be specified without this flag.
        /// ACM_STREAMCONVERTF_END : ACM conversion stream should begin returning pending instance data. For example, if a conversion stream holds instance data, such as the end of an echo filter operation, this flag will cause the stream to start returning this remaining data with optional source data. This flag can be specified with the ACM_STREAMCONVERTF_START flag.
        /// ACM_STREAMCONVERTF_START : ACM conversion stream should reinitialize its instance data. For example, if a conversion stream holds instance data, such as delta or predictor information, this flag will restore the stream to starting defaults. This flag can be specified with the ACM_STREAMCONVERTF_END flag.
        /// </param>
        /// <remarks>
        /// You must use the acmStreamPrepareHeader function to prepare the source and destination buffers before they are passed to acmStreamConvert.
        /// If an asynchronous conversion request is successfully queued by the ACM or driver and the conversion is later determined to be impossible, the ACMSTREAMHEADER structure is posted back to the application's callback function with the cbDstLengthUsed member set to zero.
        /// </remarks>
        /// <returns></returns>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern uint acmStreamConvert(IntPtr has, [In, Out] ref ACMSTREAMHEADER pash, uint fdwConvert);

        /// <summary>
        /// The acmMetrics function returns various metrics for the ACM or related ACM objects.
        /// </summary>
        /// <param name="hao">Handle to the ACM object to query for the metric specified in uMetric. For some queries, this parameter can be NULL.</param>
        /// <param name="uMetric"></param>
        /// <param name="pMetric"></param>
        /// <returns>
        /// ACMERR_NOTPOSSIBLE
        /// MMSYSERR_INVALHANDLE
        /// MMSYSERR_INVALPARAM
        /// MMSYSERR_NOTSUPPORTED
        /// </returns>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern uint acmMetrics(IntPtr hao, uint uMetric, out IntPtr pMetric);

        /// <summary>
        /// The acmStreamSize function returns a recommended size for a source or destination buffer on an ACM stream.
        /// </summary>
        /// <param name="phas"></param>
        /// <param name="cbInput">Size, in bytes, of the source or destination buffer. The fdwSize flags specify what the input parameter defines. This parameter must be nonzero.</param>
        /// <param name="pdwOutputBytes">Pointer to a variable that contains the size, in bytes, of the source or destination buffer. The fdwSize flags specify what the output parameter defines. If the acmStreamSize function succeeds, this location will always be filled with a nonzero value.</param>
        /// <param name="fdwSize"></param>
        /// <returns></returns>
        /// <remarks>
        /// An application can use this function to determine suggested buffer sizes for either source or destination buffers. The buffer sizes returned might be only an estimation of the actual sizes required for conversion. Because actual conversion sizes cannot always be determined without performing the conversion, the sizes returned will usually be overestimated.
        /// In the event of an error, the location pointed to by pdwOutputBytes will receive zero.This assumes that the pointer specified by pdwOutputBytes is valid.
        /// </remarks>
        [DllImport("Msacm32", CallingConvention = CallingConvention.StdCall)]
        public static extern uint acmStreamSize(IntPtr phas, uint cbInput, out uint pdwOutputBytes, uint fdwSize);

        #endregion
    }
}