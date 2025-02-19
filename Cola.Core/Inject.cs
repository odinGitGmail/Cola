using Cola.Authen.Jwt;
using Cola.Console;
using Cola.Exception;
using Cola.Log;
using Cola.SnowFlake;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cola.Core;

public static class Inject
{
    /// <summary>
    /// AddColaCore - 基础组件注入. 需要配置 colaLog swagger jwt 
    /// </summary>
    /// <param name="services">services.</param>
    /// <param name="configurationManager">configurationManager.</param>
    /// <returns></returns>
    public static IServiceCollection AddColaCore(this IServiceCollection services)
    {
        // 注入控制台扩展
        services
            .AddHttpContextAccessor()
            // 控制台扩展组件
            .AddSingletonColaConsole()
            // SnowFlake组件
            .AddSingletonSnowFlake()
            // 日志组件
            .AddSingletonColaLogs()
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
        return services;
    }
}