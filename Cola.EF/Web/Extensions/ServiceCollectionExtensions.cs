using Cola.EF.Core.Interfaces;
using Cola.EF.SqlSugar;
using Cola.EF.SqlSugar.Context;
using Cola.Models.Core.Models.ColaEF;
using Cola.Utils.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cola.EF.Web.Extensions;

public static class ServiceCollectionExtensions
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