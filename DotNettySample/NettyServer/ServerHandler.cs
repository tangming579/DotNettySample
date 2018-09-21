using DotNetty.Handlers.Flow;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;

namespace NettyServer
{
    public class ServerHandler : FlowControlHandler
    {
        //客户端超时次数
        private const int MAX_OVERTIME = 3;  //超时次数超过该值则注销连接


        public static IChannelHandlerContext Current;
        public override void ChannelActive(IChannelHandlerContext context)
        {
            MainWindow.SetText(@"--- Server is active ---");
            Current = context;
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            MainWindow.SetText($"--- {context.Name} is inactive ---");
        }

        public override void ChannelRead(IChannelHandlerContext context, object msg)
        {
            var buffer = msg as IByteBuffer;
            if (buffer != null)
            {
                MainWindow.SetText(@"Received from client: " + buffer.ToString(Encoding.UTF8));
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            base.UserEventTriggered(context, evt);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            MainWindow.SetText("Server Exception: " + exception);
            //context.CloseAsync();
        }

        //处理心跳包
        private void handleHeartbreat(IChannelHandlerContext context, Socket packet)
        {
            // 将心跳丢失计数器置为0
        }

        //处理数据包
        private void handleData(IChannelHandlerContext context, SendPacketsElement packet)
        {
            // 将心跳丢失计数器置为0

        }
    }
}
