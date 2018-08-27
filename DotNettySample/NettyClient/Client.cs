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

        public void Send()
        {
            try
            {
                string Command = "Hello";
                if (string.IsNullOrEmpty(Command)) return;

                IPAddress ServerIP = IPAddress.Parse("127.0.0.1"); // 服务器地址
                int ServerPort = 8888; // 服务器端口
                int ConnectTimeout = 10000; // 连接等待时间
                int ReplyTimeout = 10000;   // 回复等待时间

                // 线程池任务
                ThreadPool.QueueUserWorkItem(ThreadPoolCallback,
                    new TcpClientParams()
                    {
                        ServerIP = ServerIP,
                        ServerPort = ServerPort,
                        ConnectTimeout = ConnectTimeout,
                        ReplyTimeout = ReplyTimeout,
                        ReceiveCompletedEvent = new ManualResetEvent(false),
                        Command = Command
                    });
            }
            catch (Exception exception)
            {

            }
        }

        private void ThreadPoolCallback(object state)
        {
            TcpClientParams Args = state as TcpClientParams;
            RunClientAsync(Args).Wait();
        }

        public IChannel clientChannel;
        public async Task RunClientAsync(TcpClientParams args)
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
                    .Option(ChannelOption.ConnectTimeout, new TimeSpan(0, 0, 0, 0, args.ConnectTimeout))
                    
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                        pipeline.AddLast("echoClient", new ClientHandler());
                    }));

                clientChannel =
                    await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888));

                IByteBuffer initialMessage = Unpooled.Buffer(256);
                byte[] messageBytes = Encoding.UTF8.GetBytes("Hello world");
                initialMessage.WriteBytes(messageBytes);
                await clientChannel.WriteAndFlushAsync(initialMessage);

                // 等待回复
                if (!args.ReceiveCompletedEvent.WaitOne(args.ReplyTimeout, true))
                {
                    Console.WriteLine("Reply Timeout!");
                }

                await clientChannel.CloseAsync();
            }
            catch (Exception exp)
            {
                Console.WriteLine($"Client Exception：{exp.Message}");
            }
            finally
            {
                await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }

        public class TcpClientParams
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
}
