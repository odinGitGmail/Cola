using Newtonsoft.Json;

namespace Cola.Models.Core.Models.ColaAuthen;

/// <summary>
/// TokenModel
/// </summary>
public class TokenModel
{
    /// <summary>
    /// AccessToken
    /// </summary>
    [JsonProperty("accessToken")]
    public AccessTokenModel? AccessToken { get; set; }
}