using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kagura.Player.Base
{
    public class KContext
    {
        private static object instanceLock = new object();

        private static KContext instance;
        public static KContext Instance 
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                        {
                            instance = new KContext();
                        }
                    }
                }
                return instance;
            }
        }

        private KContext()
        {

        }
    }
}