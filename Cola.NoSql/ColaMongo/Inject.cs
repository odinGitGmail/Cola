using Cola.Console;
using Cola.Models.Core.Models.ColaMongo;
using Cola.Utils.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cola.NoSql.ColaMongo;

public static class Inject
{
    /// <summary>
    ///     inject ColaMongoInject
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddSingletonColaMongo(
        this IServiceCollection services)
    {
        var config = services.BuildServiceProvider().GetService<IConfiguration>();
        
        services.AddSingleton<IColaMongo,ColaMongo>();
        var colaConsole = services.BuildServiceProvider().GetService<IColaConsole>();
        colaConsole!.WriteInfo("注入类型【 IColaMongo, ColaMongo 】");
        return services;
    }
}
