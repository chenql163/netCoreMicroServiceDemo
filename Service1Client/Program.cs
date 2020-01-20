using System;
using System.Linq;
using System.Net;
using Com.Gshis.Services;
using Grpc.Core;
using Newtonsoft.Json;
using Consul;
using System.Threading.Tasks;

namespace Service1Client
{
    class Program
    {
        async static Task Main(string[] args)
        {
            //discover service
            ConsulClient client = new ConsulClient();
            var serviceResponse = await client.Health.Service("Service1");
            var serviceAddress = serviceResponse.Response;
            if(serviceAddress.Length <= 0)
            {
                Console.WriteLine("没有找到服务Service1，请先启动服务");
                return;
            }
            //use first address
            Channel channel = new Channel($"{serviceAddress[0].Service.Address}:{serviceAddress[0].Service.Port}", ChannelCredentials.Insecure);
            var serviceClient = new Service1.Service1Client(channel);
            var requestJsonStr = "{Name:\"server1Client\"}";
            var request = JsonConvert.DeserializeObject<HelloRequest>(requestJsonStr);

            var reply = serviceClient.Hello1(request);

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
