using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMusicCore.StreamReader
{
    /// <summary>
    /// 从Http服务器读取音频数据
    /// </summary>
    public class HttpStreamReader : IStreamReader
    {
        public override int Open()
        {
            throw new NotImplementedException();
        }

        public override int Close()
        {
            throw new NotImplementedException();
        }

        public override int Read(int size, out byte[] buffer, out int bufsize)
        {
            throw new NotImplementedException();
        }

        public override int Seek(double percent)
        {
            throw new NotImplementedException();
        }
    }
}