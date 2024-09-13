using System.Runtime.CompilerServices;
using Cola.Utils.Enums;
using Newtonsoft.Json;

namespace Cola.Models.Core.Models.ColaApiResult;

public class ApiResultError : ApiResultBase
{
    /// <summary>
    /// Message.
    /// </summary>
    [JsonProperty("message")]
    public string Message { get; set; }
}