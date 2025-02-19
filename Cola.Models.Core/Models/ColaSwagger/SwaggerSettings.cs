using System.Collections.Concurrent;

namespace Cola.Models.Core.Models.ColaSwagger;

public class SwaggerSettings
{
    public ConcurrentDictionary<string, SwaggerVersionSettings> Versions { get; set; }
}