using Cola.Utils.Constants;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

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
        });
        return services;
    }
}