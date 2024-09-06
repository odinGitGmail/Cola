using Cola.Utils.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Cola.Swagger;

public static class VersionSwaggerMiddlewareExtensions
{
    public static IApplicationBuilder UseColaSwaggerVersion(
        this IApplicationBuilder builder, ConfigurationManager configurationManager, ReflectionCache reflectionCache)
    {
        builder.UseMiddleware<SwaggerUIMiddleware>(c =>
        {
            var swaggerSection = configurationManager.GetSection(SystemConstant.CONSTANT_COLASWAGGER_SECTION);
            var swaggerTitle = swaggerSection.GetSection(SystemConstant.CONSTANT_COLASWAGGER_VERSIONTITLE_SECTION)
                .Get<string>();
            foreach (var version in reflectionCache.AllApiVersions)
            {
                c.SwaggerEndpoint($"/swagger/v{version}/swagger.json", $"{swaggerTitle} V{version}");
            }
        });
        return builder.UseMiddleware<VersionSwaggerMiddleware>();
    }
}