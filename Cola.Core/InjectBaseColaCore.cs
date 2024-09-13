using Cola.Authen.Jwt;
using Cola.Console;
using Cola.Exception;
using Cola.Log;
using Cola.Swagger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cola.Core;

public static class InjectBaseColaCore
{
    /// <summary>
    /// AddColaCore - 基础组件注入. 需要配置 colaLog swagger jwt 
    /// </summary>
    /// <param name="services">services.</param>
    /// <param name="configurationManager">configurationManager.</param>
    /// <returns></returns>
    public static IServiceCollection AddColaCore(this IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        // 注入控制台扩展
        services
            // 控制台扩展组件
            .AddSingletonColaConsole()
            // 日志组件
            .AddSingletonColaLogs(configurationManager)
            // 异常处理组件
            .AddColaExceptionSingleton();

        return services;
    }
    
    /// <summary>
    /// AddColaCore - 基础组件注入. 需要配置 colaLog jwt 
    /// </summary>
    /// <param name="services">services.</param>
    /// <param name="configurationManager">configurationManager.</param>
    /// <returns></returns>
    public static IServiceCollection AddColaSwaggerAndJwt(this IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        services
            .AddColaSwagger(configurationManager)
            .AddColaJwt(configurationManager);

        return services;
    }
}