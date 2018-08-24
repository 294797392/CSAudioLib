using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MiniMusicCore.StreamReader
{
    /// <summary>
    /// 从本地文件读取音频流
    /// </summary>
    public class FileStreamReader : IStreamReader
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("FileAudioStream");

        private FileStream stream;

        public override int Open()
        {
            if (!File.Exists(base.Uri))
            {
                return ResponseCode.SB_FILE_NOT_FOUND;
            }

            try
            {
                this.stream = new FileStream(base.Uri, FileMode.Open);
            }
            catch (Exception ex)
            {
                logger.Error("打开文件异常", ex);
                return ResponseCode.SB_OPEN_FILE_FAILED;
            }

            return ResponseCode.Success;
        }

        public override int Close()
        {
            if (this.stream != null)
            {
                this.stream.Close();
                this.stream.Dispose();
                this.stream = null;
            }

            return ResponseCode.Success;
        }

        public override int Read(int size, out byte[] buffer, out int bufsize)
        {
            buffer = new byte[size];
            bufsize = this.stream.Read(buffer, 0, size);
            return bufsize == 0 ? ResponseCode.SB_END_OF_STREAM : ResponseCode.Success;
        }

        public override int Seek(double percent)
        {
            if (percent > 100)
            {
                return ResponseCode.SB_INVALID_PARAM;
            }

            int target = (int)(this.stream.Length * (percent / 100));

            this.stream.Seek(target, SeekOrigin.Begin);

            return ResponseCode.Success;
        }
    }
}