using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Kagura.Player.Base
{
    public class Utils
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

        /// <summary>
        /// 获取source里第index位的值
        /// </summary>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool GetBit(uint source, int index)
        {
            return (source & ((uint)1 << index)) > 0;
        }
    }
}
