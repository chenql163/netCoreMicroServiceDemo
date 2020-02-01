using System;
using System.Linq;
using System.Net;
using Grpc;
using Consul;
using Grpc.Core;
using Com.Gshis.Services;

namespace LoginDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var needDeregisterServicesId = "";
            try
            {
                var firstIp = GetFirstIp4();
                var server = new Server
                {
                    Ports = { new ServerPort(firstIp, 0, ServerCredentials.Insecure) },
                    Services = { LoginService.BindService(new LoginServiceImp()) }
                };
                server.Start();
                //register services
                var port = server.Ports.First().BoundPort;
                ConsulClient client = new ConsulClient();
                var service = new AgentServiceRegistration
                {
                    ID = $"Login_{firstIp}:{port}",
                    Name = "Login",
                    Address = firstIp,
                    Port = port,
                    Check = new AgentServiceCheck
                    {
                        Interval = TimeSpan.FromSeconds(5),
                        Timeout = TimeSpan.FromSeconds(3),
                        TCP = $"{firstIp}:{port}"
                    }
                };
                client.Agent.ServiceRegister(service);
                needDeregisterServicesId = service.ID;
                Console.WriteLine($"登录服务已经在{firstIp}:{port}上监听");
                Console.WriteLine("按回车键退出程序");
                Console.ReadLine();
            }
            finally
            {
                if (!string.IsNullOrEmpty(needDeregisterServicesId))
                {
                    ConsulClient client = new ConsulClient();
                    client.Agent.ServiceDeregister(needDeregisterServicesId);
                }
            }
        }
        private static string GetFirstIp4()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] iPAddresses = Dns.GetHostAddresses(hostName);
            return iPAddresses.First(w => w.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
        }
    }
}
