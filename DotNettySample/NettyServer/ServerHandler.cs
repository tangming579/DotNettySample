using DotNetty.Handlers.Flow;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NettyServer
{
    public class ServerHandler : FlowControlHandler
    {
        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
        }

        public override void ChannelRead(IChannelHandlerContext context, object msg)
        {
            base.ChannelRead(context, msg);
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            base.UserEventTriggered(context, evt);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            base.ExceptionCaught(context, exception);
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
