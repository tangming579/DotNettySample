using DotNetty.Handlers.Flow;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;

namespace NettyClient
{
    public class ClientHandler : FlowControlHandler
    {
        readonly IByteBuffer initialMessage;

        public ClientHandler()
        {
            this.initialMessage = Unpooled.Buffer(256);
            byte[] messageBytes = Encoding.UTF8.GetBytes("Hello world");
            this.initialMessage.WriteBytes(messageBytes);
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            Console.WriteLine(@"--- Client is active ---");
            //context.WriteAndFlushAsync(this.initialMessage);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            Console.WriteLine(@"--- Client is inactive ---");
        }

        public override void ChannelRead(IChannelHandlerContext context, object msg)
        {
            var byteBuffer = msg as IByteBuffer;
            if (byteBuffer != null)
            {
                Console.WriteLine("Received from server: " + byteBuffer.ToString(Encoding.UTF8));
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
            Console.WriteLine("Client Exception: " + exception);
            context.CloseAsync();
        }

        //发送心跳包
        private void sendHeartbeatPacket(IChannelHandlerContext context)
        {
            
        }
    }
}
