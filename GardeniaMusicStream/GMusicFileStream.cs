using GMusicCore;
using MiniMusicCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GMusicStream
{
    public class GMusicFileStream : IGMusicStream
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("GMusicFileStream");

        private FileStream stream;

        public GMusicFileStream(MusicSource source) : base(source)
        {
        }

        public override GMusicStreamType StreamType
        {
            get
            {
                return GMusicStreamType.File;
            }
        }

        public override long TotalLength
        {
            get
            {
                return this.stream.Length;
            }
        }

        public override string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool EOF
        {
            get
            {
                return this.stream.Position == this.stream.Length;
            }
        }

        public override bool IsProtocolSupported(string protocol)
        {
            throw new NotImplementedException();
        }

        public override int Open()
        {
            string filePath = base.MusicSource.Uri;
            if (!File.Exists(filePath))
            {
                logger.ErrorFormat("文件不存在:{0}", filePath);
                return ResponseCode.FILE_NOT_FOUND;
            }

            this.stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            return ResponseCode.SUCCESS;
        }

        public override int Read(byte[] buff, int size)
        {
            return this.stream.Read(buff, 0, size);
        }

        public override bool ReadByte(out byte data)
        {
            data = 0;
            int len = this.stream.ReadByte();
            if (len == -1)
            {
                return false;
            }

            data = (byte)len;

            return true;
        }

        public override void Skip(int size)
        {
            this.stream.Seek(size, SeekOrigin.Current);
        }

        public override void Close()
        {
            if (this.stream != null)
            {
                try
                {

                    this.stream.Close();
                }
                catch (Exception e)
                { }
            }
        }
    }
}