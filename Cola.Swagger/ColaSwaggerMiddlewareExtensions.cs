using Cola.Utils.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cola.Swagger;

public static class ColaSwaggerMiddlewareExtensions
{
    public static IApplicationBuilder UseColaSwagger(this WebApplication app,WebApplicationBuilder builder,ConfigurationManager configurationManager)
    {
        var reflectionCache = builder.Services.BuildServiceProvider().GetService<ReflectionCache>();
        var colaSwaggerConfig = configurationManager.GetSection(SystemConstant.CONSTANT_COLASWAGGER_SECTION);
        var swaggerTitle = colaSwaggerConfig.GetSection(SystemConstant.CONSTANT_COLASWAGGER_VERSIONTITLE_SECTION)
            .Get<string>();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            foreach (var version in reflectionCache.AllApiVersions)
            {
                c.SwaggerEndpoint($"/swagger/v{version}/swagger.json", $"{swaggerTitle} V{version}");
            }
        });
        return app.UseMiddleware<ColaSwaggerMiddleware>();
    }
}