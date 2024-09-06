using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cola.Swagger;

public class VersionSwaggerMiddleware(
    RequestDelegate next,
    ReflectionCache reflectionCache)
{
    public async Task Invoke(HttpContext context)
    {
        await HandleVersionSwagger(context, reflectionCache);
        await next.Invoke(context);
    }
    
    private async Task HandleVersionSwagger(HttpContext context, ReflectionCache reflectionCache)
    {
        // Do work that doesn't write to the Response.
        if (context.Request.Path.HasValue &&
            context.Request.Path.Value.StartsWith("/api/"))
        {
            // arr as this:
            // api, version, controller, action
            var arr = context.Request.Path.Value.Split("/")
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();
            var version = arr[1];
            var controller = arr[2];
            var action = arr[3];

            // trying to get all actions with this name
            var realAction = reflectionCache.AllControllers.FirstOrDefault(x => x.Name == $"{controller}Controller")!
                .GetMethods()
                .Where(methodInfo => methodInfo.IsPublic &&
                                     methodInfo.GetCustomAttribute<ApiVersionAttribute>() != null &&
                                     Convert.ToDouble(methodInfo.GetCustomAttribute<ApiVersionAttribute>()!.Versions.FirstOrDefault()?.ToString()) <= Convert.ToDouble(version.TrimStart('v')) &&
                                     (methodInfo.Name == action || methodInfo.GetCustomAttribute<ActionNameAttribute>()?.Name == action))
                .OrderByDescending(x => x.GetCustomAttribute<ApiVersionAttribute>()!.Versions.FirstOrDefault()?.ToString())
                .First();
            var realVersion = $"{realAction.GetCustomAttribute<ApiVersionAttribute>()!.Versions.FirstOrDefault()!.ToString()}";

            if (realAction != null)
            {
                context.Request.Path = new PathString($"/api/v{realVersion}/{controller}/{realAction.Name}");
            }
        }
    }
}