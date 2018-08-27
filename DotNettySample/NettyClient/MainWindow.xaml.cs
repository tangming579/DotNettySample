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

namespace NettyClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Client.Instance.Send();
        }

        private void btnSendClear_Click(object sender, RoutedEventArgs e)
        {
            txbSend.Text = string.Empty;
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (!Client.Instance.clientChannel.Active || !Client.Instance.clientChannel.IsWritable) return;
            IByteBuffer initialMessage = Unpooled.Buffer(256);
            byte[] messageBytes = Encoding.UTF8.GetBytes(txbSend.Text);
            initialMessage.WriteBytes(messageBytes);
            Client.Instance.clientChannel.WriteAndFlushAsync(initialMessage);
        }

        private void btnRecClear_Click(object sender, RoutedEventArgs e)
        {
            txbReceive.Text = String.Empty;
        }
    }
}
