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
using DotNetty.Transport.Channels.Groups;

namespace NettyServer
{
    //管道处理类
    public class ServerHandler : FlowControlHandler
    {
        //客户端超时次数
        private const int MAX_OVERTIME = 3;  //超时次数超过该值则注销连接


        public static IChannelHandlerContext Current;
        //服务启动
        public override void ChannelActive(IChannelHandlerContext context)
        {
            MainWindow.SetText(@"--- Server is active ---");
            Current = context;
        }
        //服务关闭
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            MainWindow.SetText($"--- {context.Name} is inactive ---");
        }
        //收到消息
        public override void ChannelRead(IChannelHandlerContext context, object msg)
        {
            var buffer = msg as IByteBuffer;
            if (buffer != null)
            {
                MainWindow.SetText(@"Received from client: " + buffer.ToString(Encoding.UTF8));
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
        //客户端长时间没有Write，会触发此事件
        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            base.UserEventTriggered(context, evt);
        }
        //捕获异常
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            MainWindow.SetText("Server Exception: " + exception);
            //context.CloseAsync();
        }
        //客户端连接
        public override void HandlerAdded(IChannelHandlerContext context)
        {
            Console.WriteLine($"Client {context} is Connected!");
            base.HandlerAdded(context);
        }

        //客户端断开
        public override void HandlerRemoved(IChannelHandlerContext context)
        {
            Console.WriteLine($"Client {context} is Disconnected.");
            base.HandlerRemoved(context);
        }        
    }
}
