using System.Collections.Generic;

namespace Cola.Models.Core.Models.ColaEf;

public class ColaOrmConfigOption
{
    public string TenantType { get; set; } = "ConfigId";
    public string TenantResolutionStrategy { get; set; } = "NoTenant";
    public List<ColaEfConfig>? ColaOrmConfig { get; set; }
}