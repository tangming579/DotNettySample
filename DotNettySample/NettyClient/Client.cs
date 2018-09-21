using DotNetty.Codecs;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;

namespace NettyClient
{
    public class Client
    {
        #region Instance
        private static Client server = new Client();
        public static Client Instance => server;

        private Client()
        {

        }
        #endregion

        private ManualResetEvent ClosingArrivedEvent = new ManualResetEvent(false);

        public void Start()
        {
            try
            {
                string Command = "Hello";
                if (string.IsNullOrEmpty(Command)) return;

                IPAddress ServerIP = IPAddress.Parse("127.0.0.1"); // 服务器地址
                int ServerPort = 8007; // 服务器端口
                int ConnectTimeout = 10000; // 连接等待时间
                int ReplyTimeout = 10000;   // 回复等待时间


                var param = new ClientParams()
                {
                    ServerIP = ServerIP,
                    ServerPort = ServerPort,
                    ConnectTimeout = ConnectTimeout,
                    ReplyTimeout = ReplyTimeout,
                    ReceiveCompletedEvent = new ManualResetEvent(false),
                    Command = Command
                };
                Task.Run(() => RunClientAsync(param));
            }
            catch (Exception exception)
            {

            }

        }

        public IChannel clientChannel;
        public async Task RunClientAsync(ClientParams args)
        {

            var group = new MultithreadEventLoopGroup();

            X509Certificate2 cert = null;
            string targetHost = null;
            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    //.Option(ChannelOption.ConnectTimeout, new TimeSpan(0, 0, 0, 0, args.ConnectTimeout))

                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));
                        pipeline.AddLast("idleStateHandle", new IdleStateHandler(10, 0, 0));
                        pipeline.AddLast("NettyClient", new ClientHandler());
                    }));

                clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(args.ServerIP, args.ServerPort));

                //// 等待回复
                //if (!args.ReceiveCompletedEvent.WaitOne(args.ReplyTimeout, true))
                //{
                //    Console.WriteLine("Reply Timeout!");
                //}
                ClosingArrivedEvent.Reset();

                ClosingArrivedEvent.WaitOne();

                await clientChannel.CloseAsync();
            }
            catch (Exception exp)
            {
                MainWindow.SetText($"Client Exception：{exp.Message}");
            }
            finally
            {
                //await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
                ReConnectServer();
            }

        }
        //断线重连
        private void ReConnectServer()
        {
            try
            {
                Thread.Sleep(5000);
                MainWindow.SetText("客户端进行断线重连");
                Start();
            }
            catch (Exception e)
            {

            }
        }
    }
    public class ClientParams
    {
        /// <summary>
        /// 接收服务器地址
        /// </summary>
        public IPAddress ServerIP;

        /// <summary>
        /// 接收服务器端口
        /// </summary>
        public int ServerPort;

        /// <summary>
        /// 通信密钥
        /// </summary>
        public string SecretKey;

        /// <summary>
        /// 连接超时等待时间
        /// </summary>
        public int ConnectTimeout;

        /// <summary>
        /// 回复等待时间（毫秒）
        /// </summary>
        public int ReplyTimeout = -1;

        /// <summary>
        /// 回复数据接收完成事件
        /// </summary>
        public ManualResetEvent ReceiveCompletedEvent;

        /// <summary>
        /// 命令字符串
        /// </summary>
        public string Command;
    }
}
