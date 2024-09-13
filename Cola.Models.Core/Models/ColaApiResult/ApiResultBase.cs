using System.Text.Json.Serialization;
using Cola.Models.Core.Models.ColaAuthen;
using Cola.Utils.Enums;
using Newtonsoft.Json;

namespace Cola.Models.Core.Models.ColaApiResult;

public class ApiResultBase : IApiResult
{
    /// <summary>
    /// Message.
    /// </summary>
    [JsonProperty("message")]
    public string Message { get; set; }
    /// <summary>
    /// Code.
    /// </summary>
    [JsonProperty("code")]
    public string Code { get; set; } = EnumException.Zero.Id;
    /// <summary>
    /// StatusCode.
    /// </summary>
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }
    /// <summary>
    /// Token.
    /// </summary>
    [JsonProperty("token")]
    public TokenModel? Token { get; set; }
    
}