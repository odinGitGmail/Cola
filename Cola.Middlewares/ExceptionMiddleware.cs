using Cola.Log.Core;
using Cola.Models.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Cola.Middlewares;

public class ExceptionMiddleware(
    RequestDelegate next,
    IHostEnvironment environment,
    IColaLog colaLog)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception e)
        {
            await HandleException(context, e);
        }
    }

    private async Task HandleException(HttpContext context, Exception e)
    {
        var apiResult = new ApiResult<Object>()
        {
            Success =false
        };
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/json;charset=utf-8;";

        if (environment.IsDevelopment())
        {
            colaLog.Error(e);
            apiResult.Message = e.Message;
        }
        else
            apiResult.Message = "抱歉，出错了";

        apiResult.RequestPath = context.Request.Path.Value;
        await context.Response.WriteAsync(JsonConvert.SerializeObject(apiResult));
    }
}