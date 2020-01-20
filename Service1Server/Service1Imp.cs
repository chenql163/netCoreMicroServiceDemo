using System.Threading.Tasks;
using Com.Gshis.Services;
using Grpc.Core;

namespace Service1Server
{
    public class Service1Imp:Service1.Service1Base
    {
        public override Task<HelloResponse> Hello1(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloResponse { Message = $"hello1 {request.Name} from service1" });
        }
    }
}
