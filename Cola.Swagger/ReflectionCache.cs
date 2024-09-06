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
            .Where(x => typeof(ControllerBase).IsAssignableFrom(x));
        AllApiVersions = this.AllControllers.SelectMany(x => x.GetMethods()
                .Where(x => x.IsPublic && x.GetCustomAttribute<ApiVersionAttribute>() != null)
                .SelectMany(x => x.GetCustomAttribute<ApiVersionAttribute>().Versions))
            .GroupBy(x => x.ToString())
            .Select(x => x.Key);
    }
}