using Cola.Console;
using Cola.Models.Core.Models.ColaSwagger;
using Cola.Utils.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Cola.Swagger;

public static class Inject
{
    public static IServiceCollection AddColaSwagger(this IServiceCollection services)
    {
        var colaConsole = services.BuildServiceProvider().GetService<IColaConsole>();
        var config = services.BuildServiceProvider().GetService<IConfiguration>();
        services.Configure<SwaggerSettings>(config!.GetSection(SystemConstant.CONSTANT_COLASWAGGER_SECTION));
        services.AddSwaggerGen();
        services.ConfigureOptions<ConfigureSwaggerOptions>();
        colaConsole!.WriteInfo("注入【 ColaSwagger 】");
        return services;
    }
    

    public static IServiceCollection AddColaApiVersioning(this IServiceCollection services)
    {
        var colaConsole = services.BuildServiceProvider().GetService<IColaConsole>();
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });
        
        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
        colaConsole!.WriteInfo("注入【 ApiVersioning 】");
        return services;
    }
}