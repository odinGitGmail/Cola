using Cola.Utils.Constants;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Cola.Swagger;

public static class InjectColaSwagger
{
    public static IServiceCollection AddColaSwaager(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        var config = 
        services.AddSingleton<ReflectionCache>(new ReflectionCache());
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = false;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });
        services.AddSwaggerGen(c =>
        {
            var reflectionCache = services.BuildServiceProvider().GetService<ReflectionCache>();
            // swagger 默认自动替换版本号
            c.OperationFilter<RemoveVersionFromParameter>();
            c.DocumentFilter<ReplaceVersionWithExactValueInPath>();
            var swaggerSection = configurationManager.GetSection(SystemConstant.CONSTANT_COLASWAGGER_SECTION);
            var swaggerTitle = swaggerSection.GetSection(SystemConstant.CONSTANT_COLASWAGGER_VERSIONTITLE_SECTION)
                .Get<string>();
            foreach (var version in reflectionCache.AllApiVersions)
            {
                c.SwaggerDoc($"v{version}", new OpenApiInfo() { Title = $"{swaggerTitle}", Version = $"v{version}" });
            }
        });
        return services;
    }
}