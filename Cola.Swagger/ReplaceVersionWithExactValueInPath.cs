using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cola.Swagger;

/// <summary>
/// swagger 文档路径自动填写版本号
/// </summary>
public class ReplaceVersionWithExactValueInPath(ReflectionCache reflectionCache) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var newPaths = new OpenApiPaths();
        foreach (var item in swaggerDoc.Paths)
        {
            var arr = item.Key.Split('/');
            // route as /api/[controller]/[action] mode
            if (reflectionCache.AllControllers.Any(x => x.Name == $"{arr[^2]}Controller"))
            {
                var methods = reflectionCache.AllControllers.FirstOrDefault(x => x.Name == $"{arr[^2]}Controller")!
                    .GetMethods();
                var action = arr[arr.Length - 1];

                var version = "v" + methods
                    .FirstOrDefault(methodInfo => methodInfo.Name == action &&
                                                  methodInfo.IsPublic &&
                                                  methodInfo.GetCustomAttribute<ApiVersionAttribute>() != null)!
                    .GetCustomAttribute<ApiVersionAttribute>()?.Versions
                    .FirstOrDefault();
                var settedAction = methods
                    .FirstOrDefault(methodInfo => methodInfo.Name == action &&
                                                  methodInfo.IsPublic &&
                                                  methodInfo.GetCustomAttribute<ApiVersionAttribute>() != null)!
                    .GetCustomAttribute<ActionNameAttribute>()?.Name;
                action = settedAction ?? action;

                if (swaggerDoc.Info.Version == version)
                {
                    newPaths.Add($"/api/{version}/{arr[^2]}/{action}", item.Value);
                }
            }
        }

        swaggerDoc.Paths = newPaths;
    }
}