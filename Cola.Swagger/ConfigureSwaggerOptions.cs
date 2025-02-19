using Cola.Models.Core.Models.ColaSwagger;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cola.Swagger;

public class ConfigureSwaggerOptions(
    IApiVersionDescriptionProvider provider,
    IOptions<SwaggerSettings> swaggerSettings)
    : IConfigureOptions<SwaggerGenOptions>
{
    private readonly SwaggerSettings _swaggerSettings = swaggerSettings.Value;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            // 获取对应版本的配置
            if (_swaggerSettings.Versions.TryGetValue(description.ApiVersion.ToString(), out var versionSettings))
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo
                    {
                        Title = versionSettings.Title,
                        Version = description.ApiVersion.ToString(),
                        Description = versionSettings.Description,
                        Contact = new OpenApiContact
                        {
                            Name = versionSettings.Contact.Name,
                            Email = versionSettings.Contact.Email
                        }
                    }
                );
            }
        }
    }
}