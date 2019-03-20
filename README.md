## 一个基于WPF + DotNetty的TCP长连接小示例

DotNetty是微软团队参考Java上的Netty开发的网络通信框架，并且保留了Netty原来绝大部分的编程接口。但目前最大的问题是没有官方说明文档，官方示例也仅仅是控制台应用程序，参考价值较低。本项目展示了在WPF中DotNetty的基本使用方法。

DotNetty的GitHub地址：https://github.com/azure/dotnetty

**实现功能：**

- 心跳检测
- 断线重连
- Protoco Buffer序列化

**引用类库：**

- DotNetty.Buffers：对内存缓冲区管理的封装
- DotNetty.Codecs：对编解码的封装，包括一些基础基类的实现
- DotNetty.Common：公共的类库项目，包装线程池，并行任务和常用帮助类的封装
- DotNetty.Handlers：封装了常用的管道处理器，比如tls编解码，超时机制，心跳检查，日志等
- DotNetty.Transport：DotNetty核心的实现
- protobuf-net：ProtoBuf .Net操作类库

更高级的应用请参考《Netty权威指南》，或者使用另一个比较好用的.Net通讯框架SuperSocket