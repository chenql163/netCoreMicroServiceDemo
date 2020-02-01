using System;
using System.Threading.Tasks;
using Com.Gshis.Services;
using Grpc.Core;
using JWT.Algorithms;
using JWT.Builder;

namespace Service1Server
{
    public class Service1Imp:Service1.Service1Base
    {
        public override Task<HelloResponse> Hello1(HelloRequest request, ServerCallContext context)
        {
            //check the accessToken
            Metadata.Entry accessToken = null;
            foreach(var entry in context.RequestHeaders)
            {
                if(entry.Key.Equals("accessToken",System.StringComparison.OrdinalIgnoreCase))
                {
                    accessToken = entry;
                    break;
                }
            }
            if(accessToken == null)
            {
                context.Status = new Status(StatusCode.Unauthenticated, "没有权限访问此接口，请检查是否有传递有效的访问令牌");
                return Task.FromResult<HelloResponse>(null);
            }
            var tokenValue = new JwtBuilder()
                .WithAlgorithm(new HMACSHA512Algorithm())
                .WithSecret("jxd598@gshis.com")
                .MustVerifySignature()
                .Decode(accessToken.Value);
            Console.WriteLine($"访问令牌：{tokenValue}");
            
            //say hello
            return Task.FromResult(new HelloResponse { Message = $"hello1 {request.Name} from service1" });
        }
    }
}
