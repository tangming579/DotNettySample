using DotNetty.Handlers.Flow;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NettyClient
{
    public class ClientHandler : FlowControlHandler
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

        //发送心跳包
        private void sendHeartbeatPacket(IChannelHandlerContext context)
        {
            
        }
    }
}
