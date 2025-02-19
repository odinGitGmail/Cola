using Cola.Console;
using Cola.EF.Core.Interfaces;
using Cola.EF.SqlSugar;
using Cola.EF.SqlSugar.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Cola.EF;

public static class Inject
{
    public static IServiceCollection AddColaEF(this IServiceCollection services)
    {
        var colaConsole = services.BuildServiceProvider().GetService<IColaConsole>();
        // 注册核心服务
        services.AddSingleton<IColaDbContextFactory, ColaDbContextFactory>();
        colaConsole!.WriteInfo("注入类型【 IColaDbContextFactory, ColaDbContextFactory 】");
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        colaConsole!.WriteInfo("注入类型【 IUnitOfWork, UnitOfWork 】");
        
        return services;
    }
}