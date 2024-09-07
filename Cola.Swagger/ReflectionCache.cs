using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Cola.Swagger;

public class ReflectionCache
{
    public IEnumerable<Type> AllControllers { get; set; }

    public IEnumerable<string> AllApiVersions { get; set; }

    public ReflectionCache()
    {
        AllControllers = Assembly.GetEntryAssembly()
            .GetTypes()
            .Where(t => typeof(ControllerBase).IsAssignableFrom(t));
        AllApiVersions = this.AllControllers.SelectMany(x => x.GetMethods()
                .Where(methodInfo => methodInfo.IsPublic && methodInfo.GetCustomAttribute<ApiVersionAttribute>() != null)
                .SelectMany(methodInfo => methodInfo.GetCustomAttribute<ApiVersionAttribute>().Versions))
            .GroupBy(x => x.ToString())
            .Select(x => x.Key);
    }
}