using Newtonsoft.Json;

namespace Cola.Models.Core.Models.ColaAuthen;

/// <summary>
/// AccessTokenModel
/// </summary>
public class AccessTokenModel
{
    [JsonProperty("token")]
    public string? TokenStr { get; set; }
    [JsonProperty("expireTime")]
    public int ExpireTime { get; set; } = -1;
}