using DotNetty.Handlers.Flow;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;

namespace NettyServer
{
    public class ServerHandler : FlowControlHandler
    {
        public static IChannelHandlerContext Current;
        public override void ChannelActive(IChannelHandlerContext context)
        {
            Console.WriteLine(@"--- Server is active ---");
            Current = context;
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            Console.WriteLine($"--- {context.Name} is inactive ---");
        }

        public override void ChannelRead(IChannelHandlerContext context, object msg)
        {
            var buffer = msg as IByteBuffer;
            if (buffer != null)
            {
                Console.WriteLine(@"Received from client: " + buffer.ToString(Encoding.UTF8));
            }
            //context.WriteAsync(msg);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            base.UserEventTriggered(context, evt);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Server Exception: " + exception);
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
