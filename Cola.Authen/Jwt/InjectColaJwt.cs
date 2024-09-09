using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cola.Models.Core.Enums.Jwt;
using Cola.Models.Core.Models;
using Cola.Utils.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Cola.Authen.Jwt;

public static class InjectColaJwt
{
    public static IServiceCollection AddColaJwt(this IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        var authType = configurationManager.GetSection(SystemConstant.CONSTANT_COLAAUTH_AUTHTYPE_SECTION).Get<string>();
        var secretKey = configurationManager.GetSection(SystemConstant.CONSTANT_COLAAUTH_SECRET_SECTION).Get<string>();
        var validIssuer = configurationManager.GetSection(SystemConstant.CONSTANT_COLAAUTH_Jwt_VALIDISSUER_SECTION).Get<string>();
        var validAudience = configurationManager.GetSection(SystemConstant.CONSTANT_COLAAUTH_Jwt_AVALIDAUDIENCE_SECTION).Get<string>();
        SecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey!));
        if (authType == AuthEnumeration.Jwt.ToString())
        {
            services.AddSingleton<IAuthenToken>(new AuthenToken(configurationManager));
            services.AddAuthentication("Bearer") // 注入认证服务，认证类型：Bearer
                .AddJwtBearer(o => // 注入 Jwt Bearer认证 服务，对其进行配置
                {
                    // 对 jwt 进行配置
                    o.TokenValidationParameters = new TokenValidationParameters() // 对Token的认证是哪些参数，这里设置
                    {
                        // 这里的参数遵循 3（必要） + 2（可选） 个参数的规则
                        // 1、是否开启秘钥认证，验证秘钥
                        ValidateIssuerSigningKey = true, // 验证发行者签名秘钥
                        IssuerSigningKey = securityKey, // 发行者签名秘钥是？

                        // 2、验证发行人
                        ValidateIssuer = true, // 验证发行者
                        ValidIssuer = validIssuer, // 验证发行者的名称是？

                        // 3、验证订阅人
                        ValidateAudience = true, // 是否验证订阅者
                        ValidAudience = validAudience, // 验证订阅者的名称是？

                        // 4、过期时间 和 生命周期
                        RequireExpirationTime = true, // 使用过期时间
                        ValidateLifetime = true, // 验证生命周期

                    };

                    o.Events = new JwtBearerEvents
                    {
                        //权限验证失败后执行
                        OnChallenge = context =>
                        {
                            //终止默认的返回结果
                            context.HandleResponse();
                            string token = context.Request.Headers["Authorization"];
                            var result = JsonConvert.SerializeObject(new ApiResult<string>()
                                {Success = false, ErrorCode = "401", Message = "登录过期", RequestPath = context.Request.Path});
                            if (string.IsNullOrEmpty(token))
                            {
                                result = JsonConvert.SerializeObject(new ApiResult<string>()
                                    {Success = false, ErrorCode = "401", Message = "token不能为空", RequestPath = context.Request.Path });
                                context.Response.ContentType = "application/json";
                                //验证失败返回401
                                context.Response.StatusCode = StatusCodes.Status200OK;
                                context.Response.WriteAsync(result);
                                return Task.FromResult(result);
                            }

                            try
                            {
                                JwtSecurityTokenHandler tokenheader = new();
                                ClaimsPrincipal claimsPrincipal = tokenheader.ValidateToken(token,
                                    o.TokenValidationParameters, out SecurityToken securityToken);
                            }
                            catch (SecurityTokenExpiredException)
                            {
                                result = JsonConvert.SerializeObject(new ApiResult<string>()
                                    { Success = false, ErrorCode = "401", Message = "登录已过期", RequestPath = context.Request.Path });
                                context.Response.ContentType = "application/json";
                                //验证失败返回401
                                context.Response.StatusCode = StatusCodes.Status200OK;
                                context.Response.WriteAsync(result);
                                return Task.FromResult(result);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                result = JsonConvert.SerializeObject(new ApiResult<string>()
                                    { Success = false, ErrorCode = "401", Message = "token令牌无效", RequestPath = context.Request.Path });
                                //验证失败返回401
                                context.Response.StatusCode = StatusCodes.Status200OK;
                                context.Response.WriteAsync(result);
                                return Task.FromResult(result);
                            }

                            context.Response.ContentType = "application/json";
                            //验证失败返回401
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            context.Response.WriteAsync(result);
                            return Task.FromResult(result);
                        }
                    };
                });
        }
        else
        {
            // id4
        }
        
        return services;
    }
}