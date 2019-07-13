using Kagura.Player.Base;
using MediaCenter.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MediaCenter.AVStreams
{
    public class AVFileStream : AbstractAVStream
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("KFileStream");

        #endregion

        #region 实例变量

        private FileStream stream;

        #endregion

        #region 属性

        public override StreamType Type
        {
            get
            {
                return StreamType.File;
            }
        }

        public override long TotalLength
        {
            get
            {
                return this.stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return this.stream.Position;
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

        #endregion

        #region 公开接口

        public override bool IsSupported(string uri)
        {
            throw new NotImplementedException();
        }

        public override int Open(string uri)
        {
            string filePath = uri;
            if (!File.Exists(filePath))
            {
                logger.ErrorFormat("文件不存在:{0}", filePath);
                return ResponseCode.FILE_NOT_FOUND;
            }

            this.stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            return ResponseCode.SUCCESS;
        }

        public override int Read(byte[] buff, int count)
        {
            return this.stream.Read(buff, 0, count);
        }

        public override int Read(byte[] dst, int dstOffset, int count)
        {
            return this.stream.Read(dst, dstOffset, count);
        }

        public override int ReadByte()
        {
            return this.stream.ReadByte();
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

        #endregion
    }
}