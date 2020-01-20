using System;
using System.Linq;
using System.Net;
using Com.Gshis.Services;
using Grpc.Core;
using Newtonsoft.Json;

namespace Service1Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("请输入服务器端口:");
            string port = Console.ReadLine();
            string ip =GetFirstIp4();
            Channel channel = new Channel($"{ip}:{port}", ChannelCredentials.Insecure);
            var client = new Service1.Service1Client(channel);
            var requestJsonStr = "{Name:\"server1Client\"}";
            var request = JsonConvert.DeserializeObject<HelloRequest>(requestJsonStr);

            var reply = client.Hello1(request);

            Console.WriteLine($"Greeting :{reply.Message}");
            channel.ShutdownAsync().Wait();
            Console.WriteLine("按回车键退出");
            Console.ReadLine();
        }
        private static string GetFirstIp4()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] iPAddresses = Dns.GetHostAddresses(hostName);
            return iPAddresses.First(w => w.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
        }
    }
}
