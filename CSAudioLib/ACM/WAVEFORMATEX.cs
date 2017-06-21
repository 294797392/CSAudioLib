using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CSAudioLib.ACM
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WAVEFORMATEX 
    {
        /// <summary>
        /// Waveform-audio format type. Format tags are registered with Microsoft Corporation for many compression algorithms. A complete list of format tags can be found in the Mmreg.h header file. For one- or two-channel PCM data, this value should be WAVE_FORMAT_PCM. When this structure is included in a WAVEFORMATEXTENSIBLE structure, this value must be WAVE_FORMAT_EXTENSIBLE.
        /// </summary>
        public ushort wFormatTag;

        /// <summary>
        /// Number of channels in the waveform-audio data. Monaural data uses one channel and stereo data uses two channels.
        /// </summary>
        public ushort nChannels;

        /// <summary>
        /// Sample rate, in samples per second (hertz). If wFormatTag is WAVE_FORMAT_PCM, then common values for nSamplesPerSec are 8.0 kHz, 11.025 kHz, 22.05 kHz, and 44.1 kHz. For non-PCM formats, this member must be computed according to the manufacturer's specification of the format tag.
        /// </summary>
        public uint nSamplesPerSec;

        /// <summary>
        /// Required average data-transfer rate, in bytes per second, for the format tag. If wFormatTag is WAVE_FORMAT_PCM, nAvgBytesPerSec should be equal to the product of nSamplesPerSec and nBlockAlign. For non-PCM formats, this member must be computed according to the manufacturer's specification of the format tag.
        /// </summary>
        public uint nAvgBytesPerSec;

        /// <summary>
        /// Block alignment, in bytes. The block alignment is the minimum atomic unit of data for the wFormatTag format type. If wFormatTag is WAVE_FORMAT_PCM or WAVE_FORMAT_EXTENSIBLE, nBlockAlign must be equal to the product of nChannels and wBitsPerSample divided by 8 (bits per byte). For non-PCM formats, this member must be computed according to the manufacturer's specification of the format tag.
        /// Software must process a multiple of nBlockAlign bytes of data at a time. Data written to and read from a device must always start at the beginning of a block. For example, it is illegal to start playback of PCM data in the middle of a sample (that is, on a non-block-aligned boundary).
        /// </summary>
        public ushort nBlockAlign;

        /// <summary>
        /// Bits per sample for the wFormatTag format type. If wFormatTag is WAVE_FORMAT_PCM, then wBitsPerSample should be equal to 8 or 16. For non-PCM formats, this member must be set according to the manufacturer's specification of the format tag. If wFormatTag is WAVE_FORMAT_EXTENSIBLE, this value can be any integer multiple of 8 and represents the container size, not necessarily the sample size; for example, a 20-bit sample size is in a 24-bit container. Some compression schemes cannot define a value for wBitsPerSample, so this member can be 0.
        /// </summary>
        public ushort wBitsPerSample;

        /// <summary>
        /// Size, in bytes, of extra format information appended to the end of the WAVEFORMATEX structure. This information can be used by non-PCM formats to store extra attributes for the wFormatTag. If no extra information is required by the wFormatTag, this member must be set to 0. For WAVE_FORMAT_PCM formats (and only WAVE_FORMAT_PCM formats), this member is ignored. When this structure is included in a WAVEFORMATEXTENSIBLE structure, this value must be at least 22.
        /// </summary>
        public ushort cbSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MPEGLAYER3WAVEFORMAT
    {
        public WAVEFORMATEX wfx;
        public ushort wID;
        public uint fdwFlags;
        public ushort nBlockSize;
        public ushort nFramesPerBlock;
        public ushort nCodecDelay;
    }
}