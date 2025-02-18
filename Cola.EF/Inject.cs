using Cola.EF.Core.Interfaces;
using Cola.EF.SqlSugar;
using Cola.EF.SqlSugar.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Cola.EF;

public static class Inject
{
    public static IServiceCollection AddColaEF(this IServiceCollection services)
    {
        // 注册核心服务
        services.AddSingleton<IColaDbContextFactory, ColaDbContextFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // 多租户支持
        // services.AddScoped<ITenantProvider, HttpContextTenantProvider>();
        // services.AddScoped(typeof(IRepository<>), typeof(TenantRepository<>));
        
        // 缓存装饰器
        // services.AddMemoryCache();
        // services.Decorate(typeof(IRepository<>), typeof(CachedRepository<>));
        
        return services;
    }
}