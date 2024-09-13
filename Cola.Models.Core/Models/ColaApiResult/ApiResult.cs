using Newtonsoft.Json;

namespace Cola.Models.Core.Models.ColaApiResult;

public class ApiResult<T>:ApiResultBase
{
    /// <summary>
    /// Data.
    /// </summary>
    [JsonProperty("data")]
    public T Data { get; set; }
    
    
}