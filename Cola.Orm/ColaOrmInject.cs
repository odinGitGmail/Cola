using Cola.Console;
using Cola.Exception;
using Cola.Models.Core.Models.ColaEf;
using Cola.Models.Core.Models.ColaSqlsugar;
using Cola.Orm.Tenant;
using Cola.Utils.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace Cola.Orm;

public static class ColaOrmInject
{
    public static IServiceCollection AddColaOrm(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ColaOrmConfigOption> action,
        List<GlobalQueryFilter>? tableFilter = null,
        Action<string, SugarParameter[]>? handlerLogExecuting=null,
        Action<SqlSugarException>? handlerSqlSugarException=null)
    {
        var colaEfConfig = configuration.GetSection(SystemConstant.CONSTANT_COLAORM_SECTION).Get<ColaOrmConfigOption>();
        var opts = new ColaOrmConfigOption
        {
            TenantType = colaEfConfig!.TenantType,
            TenantResolutionStrategy = colaEfConfig!.TenantResolutionStrategy,
            ColaOrmConfig = colaEfConfig.ColaOrmConfig
        };
        return InjectSqlSugar(services, opts, tableFilter, handlerLogExecuting, handlerSqlSugarException);
    }
    
    private static IServiceCollection InjectSqlSugar(
        IServiceCollection services,
        ColaOrmConfigOption colaOrmConfigOption,
        List<GlobalQueryFilter>? tableFilter = null,
        Action<string, SugarParameter[]>? handlerLogExecuting=null,
        Action<SqlSugarException>? handlerSqlSugarException=null)
    {
        // 配置参数验证
        ValidateColaEfConfigOption(services, colaOrmConfigOption);
        
        var sqlSugarConfigLst = new List<ConnectionConfig>();
        var colaConsole = services.BuildServiceProvider().GetService<IColaConsole>();
        
        for (var i = 0; i < colaOrmConfigOption.ColaOrmConfig!.Count; i++)
        {
            var opt = colaOrmConfigOption.ColaOrmConfig[i];
            sqlSugarConfigLst.Add(new ConnectionConfig
            {
                ConfigId = opt.ConfigId,
                DbType = opt.GetSqlSugarDbType(),
                ConnectionString = opt.ConnectionString,
                IsAutoCloseConnection = opt.IsAutoCloseConnection
            });
        }

        services.AddSingleton<ISqlSugarClient>(s =>
        {
            var sqlSugarScope = new SqlSugarScope(
                sqlSugarConfigLst,
                db =>
                {
                    if (handlerLogExecuting != null)
                    {
                        foreach (var colaEfConfig in colaOrmConfigOption.ColaOrmConfig)
                        {
                            db.GetConnectionScope(colaEfConfig.ConfigId).Aop.OnLogExecuting = (sql, parameters) =>
                                handlerLogExecuting!(sql,parameters);
                        }
                    }
                    if (handlerSqlSugarException != null)
                    {
                        foreach (var colaEfConfig in colaOrmConfigOption.ColaOrmConfig)
                        {
                            db.GetConnectionScope(colaEfConfig.ConfigId).Aop.OnError = (ex) =>
                                handlerSqlSugarException!(ex);
                        }
                    }
                });
            return sqlSugarScope;
        });
        colaConsole!.WriteInfo("注入类型【 ISqlSugarClient, SqlSugarClient 】");
        InjectTenantResolutionStrategy(services, colaOrmConfigOption);
        return services;
    }
    
    private static void ValidateColaEfConfigOption(IServiceCollection services, ColaOrmConfigOption option)
    {
        var exceptionHelper = services.BuildServiceProvider().GetService<IColaException>();
        if (option.ColaOrmConfig == null || option.ColaOrmConfig.Count == 0)
            exceptionHelper!.ThrowException("ColaEfConfig 配置不正确");
    }
    
    private static void InjectTenantResolutionStrategy(IServiceCollection services, ColaOrmConfigOption colaOrmConfigOption)
    {
        var httpContextAccessor = services.BuildServiceProvider().GetService<IHttpContextAccessor>();
        var colaConsole = services.BuildServiceProvider().GetService<IColaConsole>();
        if (httpContextAccessor == null)
        {
            colaConsole!.WriteLine("SqlSugar配置不正确，无类型【 IHttpContextAccessor, httpContextAccessor 】", backgroundColor: ConsoleColor.DarkRed);
        }
        Dictionary<string, ITenantResolutionStrategy> dicTenantResolutionStrategys =
            new Dictionary<string, ITenantResolutionStrategy>()
            {
                { "DomainTenant",new DomainTenantResolutionStrategy(httpContextAccessor!, colaOrmConfigOption, services.BuildServiceProvider()) },
                { "HttpHeaderTenant",new HttpHeaderTenantResolutionStrategy(httpContextAccessor!) },
                { "RouteValueTenant",new RouteValueTenantResolutionStrategy(httpContextAccessor!) },
                { "NoTenant",new NoTenantResolutionStrategy() }
            };
        var tenantResolutionStrategys = dicTenantResolutionStrategys[colaOrmConfigOption.TenantResolutionStrategy];
        colaConsole!.WriteInfo("注入类型【 ITenantResolutionStrategy 】");
        services.AddSingleton(tenantResolutionStrategys);
    }
}