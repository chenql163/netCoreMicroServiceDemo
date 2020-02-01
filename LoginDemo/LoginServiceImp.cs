using Com.Gshis.Services;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JWT.Builder;
using JWT.Algorithms;

namespace LoginDemo
{
    public class LoginServiceImp:LoginService.LoginServiceBase
    {
        public override Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            if(request != null && "000001".Equals(request.Hid) && "admin".Equals(request.UserName) && "Jxd598".Equals(request.Password))
            {
                // calc jwt token
                var secretKey = "jxd598@gshis.com";
                var token = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA512Algorithm())
                    .WithSecret(secretKey)
                    .ExpirationTime(DateTime.Now.AddHours(8))
                    .IssuedAt(DateTime.Now)
                    .Issuer("Gemstar")
                    .AddClaim("Hid", request.Hid)
                    .AddClaim("UserCode", request.UserName)
                    .Build();

                return Task.FromResult(new LoginResponse
                {
                    Success = true,
                    ErrMsg = "",
                    Token = token
                });
            }
            return Task.FromResult(new LoginResponse
            {
                Success = false,
                ErrMsg = "请指定有效的登录参数"
            });
        }
    }
}
