using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CSAudioLib
{
    public class LibUtils
    {
        /// <summary>
        /// 结构体转byte数组, 不会释放内存
        /// </summary>
        /// <param name="structObj">要转换的结构体</param>
        /// <returns>转换后的byte数组</returns>
        public static IntPtr StructureToPtr(object structObj)
        {
            int size = Marshal.SizeOf(structObj);

            IntPtr structPtr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(structObj, structPtr, false);

            //Marshal.FreeHGlobal(structPtr);

            return structPtr;
        }

        /// <summary>
        /// 结构体转byte数组, 释放内存
        /// </summary>
        /// <returns></returns>
        public static IntPtr StructureToPtrFree(object structObj)
        {
            int size = Marshal.SizeOf(structObj);

            IntPtr structPtr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(structObj, structPtr, false);

            Marshal.FreeHGlobal(structPtr);

            return structPtr;
        }

        public static void PrintLog(string log, params object[] param)
        {
#if CSADBG
            Console.WriteLine(log, param);
#endif
        }

        /// <summary>
        /// 获取每个采样的字节数
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="bitPerSample"></param>
        /// <returns></returns>
        public static short GetBlockAlign(int channels, int bitPerSample)
        {
            return (short)(channels * bitPerSample / 8);
        }

        /// <summary>
        /// 获取声音每秒传输字节数
        /// </summary>
        /// <returns></returns>
        public static int GetBytesPerSec(short blockAlign, int samplePerSeconds)
        {
            return blockAlign * samplePerSeconds;
        }
    }
}