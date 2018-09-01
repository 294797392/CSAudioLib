using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MiniMusic
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var req = WebRequest.Create("https://www.icbc.com.cn/icbc/");
            var resp = req.GetResponse();
            byte[] s = new byte[resp.ContentLength];
            resp.GetResponseStream().Read(s, 0, s.Length);
            ss.Text = Encoding.UTF8.GetString(s);
            Console.WriteLine("");

            //var source = IAudioSource.Create("http://isure.stream.qqmusic.qq.com/C400003mIUEJ1SC3Iy.m4a?vkey=595D6BBC20A358F8C51C3D23B9B7A616A84CCCEE7025AEB474A1A980AAE0049963B09DF96E98A4D54181DE3D71488CCC3D12BC828093EA25&guid=7383191610&uin=294797392&fromtag=66");

            //int size = 16 >> 3;
        }
    }
}