using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DotNetty.Buffers;

namespace NettyServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static MainWindow Current;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Current = this;
            //Task.Run(() => Server.Instance.Start());
            Server.Instance.Start();
        }

        private void btnSendClear_Click(object sender, RoutedEventArgs e)
        {
            txbSend.Text = string.Empty;
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (ServerHandler.Current == null || ServerHandler.Current.Removed) return;
            IByteBuffer initialMessage = Unpooled.Buffer(256);
            byte[] messageBytes = Encoding.UTF8.GetBytes(txbSend.Text);
            initialMessage.WriteBytes(messageBytes);
            ServerHandler.Current.WriteAndFlushAsync(initialMessage);
        }

        private void btnRecClear_Click(object sender, RoutedEventArgs e)
        {
            txbReceive.Text=String.Empty;
        }

        public static void SetText(string msg)
        {
            if (Current == null) return;
            Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Current.txbReceive.AppendText(msg + '\n');
            }));
        }
    }
}