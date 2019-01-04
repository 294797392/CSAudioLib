using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kagura.Player.Base
{
    public partial class MPG123Parser
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("MPG123Parser");

        private IntPtr mpg123Handle = IntPtr.Zero;

        private bool OpenHandle()
        {
            if (this.mpg123Handle == IntPtr.Zero)
            {
            }
            return true;
        }
    }
}
