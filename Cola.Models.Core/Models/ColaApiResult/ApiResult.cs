using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Cola.Models.Core.Models.ColaApiResult;

public class ApiResult<T> : ApiResultBase, IActionResult
{
    private T? Data { get; set; }
    private ApiResult(int statusCode, string? message = null, T data = default(T), string? token = null, string? refreshToken = null)
    {
        StatusCode = statusCode;
        Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        Data = data;
        Token = token;
        RefreshToken = refreshToken;
    }
    
    public static ApiResult<T> Success(T data = default(T), string? token = null, string? refreshToken = null)
    {
        return new ApiResult<T>(200, "Success", data, token, refreshToken);
    }

    public static ApiResult<T> BadRequest(string message = "Bad Request", T data = default(T))
    {
        return new ApiResult<T>(400, message, data);
    }

    public static ApiResult<T> Unauthorized(string message = "Unauthorized")
    {
        return new ApiResult<T>(401, message);
    }

    public static ApiResult<T> NotFound(string message = "Resource Not Found")
    {
        return new ApiResult<T>(404, message);
    }

    public static ApiResult<T> InternalServerError(string message = "Internal Server Error")
    {
        return new ApiResult<T>(500, message);
    }
    public async Task ExecuteResultAsync(ActionContext context)
    {
        var response = new
        {
            StatusCode,
            Message,
            Data,
            Token,
            RefreshToken
        };
        
        var json = JsonConvert.SerializeObject(response);
        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.StatusCode = StatusCode;
        await context.HttpContext.Response.WriteAsync(json);
    }
    
    private static string? GetDefaultMessageForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            200 => "Success",
            400 => "Bad Request",
            401 => "Unauthorized",
            404 => "Resource Not Found",
            500 => "Internal Server Error",
            _ => null
        };
    }
}