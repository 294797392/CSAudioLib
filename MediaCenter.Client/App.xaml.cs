using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace DirectSoundLibTest
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("App");

        private static string ExternalLog4netConfig = "log4net.xml";

        static App()
        {
            try
            {
                FileInfo configFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ExternalLog4netConfig));
                if (configFile.Exists)
                {
                    log4net.Config.XmlConfigurator.ConfigureAndWatch(configFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("初始化日志异常, {0}", ex);
            }
        }
    }
}
