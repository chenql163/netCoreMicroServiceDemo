using System;
using System.Linq;
using System.Net;
using Com.Gshis.Services;
using Grpc.Core;

namespace Service1Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string ip = GetFirstIp4();

            Server server = new Server
            {
                Services = { Service1.BindService(new Service1Imp()) },
                Ports = { new ServerPort(ip, 0, ServerCredentials.Insecure) }
            };
            server.Start();
            var firstPort = server.Ports.First();
            Console.WriteLine($"Greeter server listening on {firstPort.Host} port {firstPort.BoundPort}");
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
        private static string GetFirstIp4()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] iPAddresses = Dns.GetHostAddresses(hostName);
            return iPAddresses.First(w => w.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
        }
    }
}
