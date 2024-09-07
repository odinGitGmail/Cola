using System.Reflection;
using Cola.Models.Core.Enums.Jwt;
using Cola.Utils.Constants;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Cola.Swagger;

public static class InjectColaSwagger
{
    public static IServiceCollection AddColaSwagger(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = false;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });
        var reflectionCache = new ReflectionCache();
        services.AddSingleton<ReflectionCache>(reflectionCache);
        var colaSwaggerConfig = configurationManager.GetSection(SystemConstant.CONSTANT_COLASWAGGER_SECTION);
        var swaggerTitle = colaSwaggerConfig.GetSection(SystemConstant.CONSTANT_COLASWAGGER_VERSIONTITLE_SECTION)
            .Get<string>();
        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<RemoveVersionFromParameter>();
            c.DocumentFilter<ReplaceVersionWithExactValueInPath>();
            foreach (var version in reflectionCache.AllApiVersions)
            {
                c.SwaggerDoc($"v{version}", new OpenApiInfo() { Title = $"{swaggerTitle}", Version = $"v{version}" });
            }
            
            // 下面三个方法为 Swagger JSON and UI设置xml文档注释路径
            var basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
            var xmlPath = Path.Combine(basePath, $"{Assembly.GetEntryAssembly().GetName().Name}.xml");
            c.IncludeXmlComments(xmlPath);
                
            #region Jwt
            
            var authTypeSection = configurationManager.GetSection(SystemConstant.CONSTANT_COLAAUTH_AUTHTYPE_SECTION);
            if (authTypeSection.Get<string>()==AuthEnumeration.Jwt.ToString())
            {
                //开启权限小锁
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传递)直接在下面框中输入Bearer {token}(注意两者之间是一个空格)",
                    Name = "Authorization", //jwt默认的参数名称
                    In = ParameterLocation.Header, //jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
            
                // 安全要求
                var securityScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                };
                var securityRequirement = new OpenApiSecurityRequirement { { securityScheme, new string[] { } } };
                c.AddSecurityRequirement(securityRequirement);
            }
            
            #endregion
            
        });
        return services;
    }
}