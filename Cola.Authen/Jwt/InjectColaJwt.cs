using System.Text;
using Cola.Models.Core.Enums.Jwt;
using Cola.Utils.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
            services.AddAuthentication("Bearer")        // 注入认证服务，认证类型：Bearer
                .AddJwtBearer(o =>        // 注入 Jwt Bearer认证 服务，对其进行配置
                {   // 对 jwt 进行配置
                    o.TokenValidationParameters = new TokenValidationParameters()        // 对Token的认证是哪些参数，这里设置
                    {
                        // 这里的参数遵循 3（必要） + 2（可选） 个参数的规则
                        // 1、是否开启秘钥认证，验证秘钥
                        ValidateIssuerSigningKey = true,        // 验证发行者签名秘钥
                        IssuerSigningKey = securityKey,        // 发行者签名秘钥是？

                        // 2、验证发行人
                        ValidateIssuer = true,        // 验证发行者
                        ValidIssuer = validIssuer,        // 验证发行者的名称是？

                        // 3、验证订阅人
                        ValidateAudience = true,        // 是否验证订阅者
                        ValidAudience = validAudience,        // 验证订阅者的名称是？

                        // 4、过期时间 和 生命周期
                        RequireExpirationTime = true,        // 使用过期时间
                        ValidateLifetime = true,        // 验证生命周期
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