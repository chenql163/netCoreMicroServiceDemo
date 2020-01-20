using System;
using System.Linq;
using System.Net;
using Com.Gshis.Services;
using Grpc.Core;
using Consul;

namespace Service1Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string needDeregisterServiceId="";
            try
            {
                //start service
                string ip = GetFirstIp4();

                Server server = new Server
                {
                    Services = { Service1.BindService(new Service1Imp()) },
                    Ports = { new ServerPort(ip, 0, ServerCredentials.Insecure) }
                };
                server.Start();
                var firstPort = server.Ports.First().BoundPort;
                //register service
                ConsulClient client = new ConsulClient();
                var serviceInfo = new AgentServiceRegistration
                {
                    ID = $"Service1_{ip}:{firstPort}",
                    Name = $"Service1",
                    Address = ip,
                    Port = firstPort,
                    Check = new AgentServiceCheck
                    {
                        Interval = TimeSpan.FromSeconds(5),
                        Timeout = TimeSpan.FromSeconds(3),
                        TCP = $"{ip}:{firstPort}"
                    }
                };
                client.Agent.ServiceRegister(serviceInfo);
                needDeregisterServiceId = serviceInfo.ID;

                Console.WriteLine($"Greeter server listening on {ip} port {firstPort}");
                Console.WriteLine("Press any key to stop the server...");
                Console.ReadKey();

                server.ShutdownAsync().Wait();
            } finally
            {
                //deregister service
                if (!string.IsNullOrEmpty(needDeregisterServiceId))
                {
                    ConsulClient client = new ConsulClient();
                    client.Agent.ServiceDeregister(needDeregisterServiceId);
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
