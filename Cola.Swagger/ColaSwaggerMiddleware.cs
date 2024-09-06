using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cola.Swagger;

public class ColaSwaggerMiddleware(
    RequestDelegate next,
    ReflectionCache reflectionCache)
{
    public async Task Invoke(HttpContext context)
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
            var realAction = reflectionCache.AllControllers.FirstOrDefault(x => x.Name == $"{controller}Controller")
                .GetMethods()
                .Where(x => x.IsPublic &&
                            x.GetCustomAttribute<ApiVersionAttribute>() != null &&
                            Convert.ToDouble(x.GetCustomAttribute<ApiVersionAttribute>().Versions.FirstOrDefault()?.ToString()) <= Convert.ToDouble(version.TrimStart('v')) &&
                            (x.Name == action || x.GetCustomAttribute<ActionNameAttribute>()?.Name == action))
                .OrderByDescending(x => x.GetCustomAttribute<ApiVersionAttribute>().Versions.FirstOrDefault()?.ToString())
                .First();
            var realVersion = $"{realAction.GetCustomAttribute<ApiVersionAttribute>().Versions.FirstOrDefault()?.ToString()}";

            if (realAction != null)
            {
                context.Request.Path = new PathString($"/api/v{realVersion}/{controller}/{realAction.Name}");
            }
        }

        // Do logging or other work that doesn't write to the Response.
        await next.Invoke(context);
    }
}