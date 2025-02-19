using System.Text;
using Cola.Console;
using Cola.Utils.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Cola.Authen.Jwt;

public static class InjectColaJwt
{
    public static IServiceCollection AddJwtSwagger(this IServiceCollection services)
    {
        var colaConsole = services.BuildServiceProvider().GetService<IColaConsole>();
        services.AddSwaggerGen(c =>
        {
            // c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            // 添加 JWT 认证支持
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, Array.Empty<string>() }
            });
        });
        colaConsole!.WriteInfo("注入【 SwaggerGen 】");
        return services;
    }
    public static IServiceCollection AddColaJwt(this IServiceCollection services)
    {
        var config = services.BuildServiceProvider().GetService<IConfiguration>();
        var tokenParam = new TokenParameter(config);
        var colaConsole = services.BuildServiceProvider().GetService<IColaConsole>();
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenParam.GetIssuer(),
                    ValidAudience = tokenParam.GetAudience(),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenParam.GetSecret()))
                };
            });
        colaConsole!.WriteInfo("注入【 Jwt Authentication 】");
        // 添加授权服务
        services.AddAuthorization();
        services.AddSingleton<IAuthenToken, AuthenToken>();
        colaConsole!.WriteInfo("注入【 IAuthenToken, AuthenToken 】");
        return services;
        
    }
}