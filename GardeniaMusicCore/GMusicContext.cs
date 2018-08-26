using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMusicCore
{
    public class GMusicContext
    {
        private static object instanceLock = new object();

        private static GMusicContext instance;
        public static GMusicContext Instance 
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                        {
                            instance = new GMusicContext();
                        }
                    }
                }
                return instance;
            }
        }

        private GMusicContext()
        {

        }
    }
}