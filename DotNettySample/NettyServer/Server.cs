using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NettyServer
{
    public class Server
    {
        #region Instance
        private static Server server = new Server();
        public static Server Instance => server;

        private Server()
        {

        }
        #endregion

        /// <summary>
        /// 服务器是否已运行
        /// </summary>
        private bool IsServerRunning = false;
        /// <summary>
        /// 关闭侦听器事件
        /// </summary>
        private ManualResetEvent ClosingArrivedEvent = new ManualResetEvent(false);
        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            try
            {
                if (IsServerRunning)
                {
                    ClosingArrivedEvent.Set();  // 停止侦听
                }
                else
                {
                    IPAddress ServerIP = IPAddress.Parse("127.0.0.1"); // 服务器地址
                    int ServerPort = 8007; // 服务器端口
                    int Backlog = 100; // 最大连接等待数

                    //线程池任务
                    ThreadPool.QueueUserWorkItem(ThreadPoolCallback,
                        new TcpServerParams()
                        {
                            ServerIP = ServerIP,
                            ServerPort = ServerPort,
                            Backlog = Backlog
                        });
                }
            }
            catch (Exception exp)
            {

            }
        }

        private void ThreadPoolCallback(object state)
        {
            TcpServerParams Args = state as TcpServerParams;
            RunServerAsync(Args).Wait();
        }

        public async Task RunServerAsync(TcpServerParams args)
        {

            IEventLoopGroup bossGroup;
            IEventLoopGroup workerGroup;

            bossGroup = new MultithreadEventLoopGroup(1);
            workerGroup = new MultithreadEventLoopGroup();

            try
            {
                var bootstrap = new ServerBootstrap();
                    bootstrap
                    .Group(bossGroup, workerGroup)
                    .Channel<TcpServerSocketChannel>()
                    .Option(ChannelOption.SoBacklog, args.Backlog) //设置网络IO参数等
                    .Option(ChannelOption.SoKeepalive, true)//保持连接
                    .Handler(new LoggingHandler("SRV-LSTN"))//在主线程组上设置一个打印日志的处理器
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        //工作线程连接器 是设置了一个管道，服务端主线程所有接收到的信息都会通过这个管道一层层往下传输
                        //同时所有出栈的消息 也要这个管道的所有处理器进行一步步处理
                        IChannelPipeline pipeline = channel.Pipeline;

                        //IdleStateHandler 心跳
                        pipeline.AddLast(new IdleStateHandler(150, 0, 0));//第一个参数为读，第二个为写，第三个为读写全部

                        //出栈消息，通过这个handler 在消息顶部加上消息的长度
                        pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));

                        //入栈消息通过该Handler,解析消息的包长信息，并将正确的消息体发送给下一个处理Handler
                        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                        pipeline.AddLast("NettyServer", new ServerHandler());
                    }));

                IChannel boundChannel = await bootstrap.BindAsync(args.ServerPort);

                //运行至此处，服务启动成功
                IsServerRunning = true;

                ClosingArrivedEvent.Reset();
                ClosingArrivedEvent.WaitOne();
                await boundChannel.CloseAsync();
            }
            finally
            {
                await Task.WhenAll(
                    bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
        }
    }

    public class TcpServerParams
    {
        public IPAddress ServerIP { get; set; }

        public int ServerPort { get; set; }

        public int Backlog { get; set; } = 100;
    }
}
