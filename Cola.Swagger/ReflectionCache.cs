using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Cola.Swagger;

/// <summary>
/// 反射缓存类 - 反射分析controller和version
/// </summary>
public class ReflectionCache
{
    public IEnumerable<Type> AllControllers { get; set; }

    public IEnumerable<string> AllApiVersions { get; set; }

    public ReflectionCache()
    {
        this.AllControllers = Assembly.GetCallingAssembly()
            .GetTypes()
            .Where(x => typeof(ControllerBase).IsAssignableFrom(x));
        this.AllApiVersions = this.AllControllers.SelectMany(x => x.GetMethods()
                .Where(methodInfo => methodInfo.IsPublic && methodInfo.GetCustomAttribute<ApiVersionAttribute>() != null)
                .SelectMany(methodInfo => methodInfo.GetCustomAttribute<ApiVersionAttribute>()!.Versions))
            .GroupBy(x => x.ToString())
            .Select(x => x.Key);
    }
}