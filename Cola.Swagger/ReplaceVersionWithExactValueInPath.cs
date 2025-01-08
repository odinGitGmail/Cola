using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cola.Swagger;

public class ReplaceVersionWithExactValueInPath : IDocumentFilter
{
    private readonly ReflectionCache _reflectionCache;
    public ReplaceVersionWithExactValueInPath(ReflectionCache reflectionCache)
    {
        _reflectionCache = reflectionCache;
    }
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var newPaths = new OpenApiPaths();
        foreach (var item in swaggerDoc.Paths)
        {
            var arr = item.Key.Split('/');
            // route as /api/[controller]/[action] mode
            if (_reflectionCache.AllControllers.Any(x => x.Name == $"{arr[^2]}Controller"))
            {
                var methods = _reflectionCache.AllControllers
                    .FirstOrDefault(x => x.Name == $"{arr[arr.Length - 2]}Controller")
                    .GetMethods();
                var action = arr[^1];

                var version = "v" + methods
                    .FirstOrDefault(x =>
                                         x.IsPublic &&
                                         x.GetCustomAttribute<ApiVersionAttribute>() != null)
                    .GetCustomAttribute<ApiVersionAttribute>()?.Versions
                    .FirstOrDefault();
                var sa = methods
                    .FirstOrDefault(x => 
                                         x.IsPublic &&
                                         x.GetCustomAttribute<ApiVersionAttribute>() != null)
                    .GetCustomAttribute<ActionNameAttribute>();
                action = sa != null ? sa.Name : action;

                if (swaggerDoc.Info.Version == version)
                {
                    newPaths.Add($"/api/{version}/{arr[^2]}/{action}", item.Value);
                }
            }
        }

        swaggerDoc.Paths = newPaths;
    }
}