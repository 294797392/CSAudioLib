using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMusic.StreamReader
{
    /// <summary>
    /// 存储音频流的信息
    /// </summary>
    public struct StreamInfo
    {
        public string StreamType { get; set; }
	
	public long TotalSeconds { get; set; }
    }
}