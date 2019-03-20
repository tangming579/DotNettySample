using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
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
            catch (Exception exception)
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
                bootstrap.Group(bossGroup, workerGroup);

                bootstrap.Channel<TcpServerSocketChannel>();

                bootstrap
                    .Option(ChannelOption.SoBacklog, args.Backlog)
                    .Handler(new LoggingHandler("SRV-LSTN"))
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                        pipeline.AddLast("NettyServer", new ServerHandler());
                    }));

                IChannel boundChannel = await bootstrap.BindAsync(args.ServerPort);

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
