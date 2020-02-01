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
            var loginServiceDiscoverResponse = await client.Health.Service("Login");
            var loginServiceAddresses = loginServiceDiscoverResponse.Response;
            if(loginServiceAddresses.Length <= 0)
            {
                Console.WriteLine("没有找到Login登录服务，请先启动服务");
                return;
            }
            //do login and get token
            var loginResponse = GetLoginToken(loginServiceAddresses[0]);
            if (!loginResponse.Success)
            {
                Console.WriteLine($"登录失败,原因：{loginResponse.ErrMsg}");
                return;
            }

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

            Metadata metadata = new Metadata();
            metadata.Add("accessToken", loginResponse.Token);
            var callOption = new CallOptions().WithHeaders(metadata);
            
            var reply = serviceClient.Hello1(request,callOption);

            Console.WriteLine($"Greeting :{reply.Message}");
            channel.ShutdownAsync().Wait();
            Console.WriteLine("按回车键退出");
            Console.ReadLine();
        }

        private static LoginResponse GetLoginToken(ServiceEntry serviceEntry)
        {
            Channel channel = new Channel($"{serviceEntry.Service.Address}:{serviceEntry.Service.Port}", ChannelCredentials.Insecure);
            var client = new LoginService.LoginServiceClient(channel);
            var loginResponse = client.Login(new LoginRequest
            {
                Hid = "000001",
                UserName = "admin",
                Password = "Jxd598"
            });
            return loginResponse;
        }

        private static string GetFirstIp4()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] iPAddresses = Dns.GetHostAddresses(hostName);
            return iPAddresses.First(w => w.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
        }
    }
}
