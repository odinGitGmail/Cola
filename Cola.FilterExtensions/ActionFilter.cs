using System.Text;
using Cola.Log.Core;
using Cola.Models.Core.Models.ColaApiResult;
using Cola.Utils.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cola.FilterExtensions;

/// <summary>
/// 方法过滤器
/// </summary>
public class ColaActionFilter(IColaLog colaLog) : IActionFilter
{

    /// <summary>
    /// 方法执行前
    /// </summary>
    /// <param name="context"></param>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (colaLog != null)
        {
            // 记录请求参数日志
            ControllerActionDescriptor desc = context.ActionDescriptor as ControllerActionDescriptor;
            if (desc != null)
            {
                var logText = CreateRequestLogText(
                    context.HttpContext.Request.Method,
                    desc.ControllerName,
                    desc.ActionName,
                    context.ActionArguments);
                colaLog.Info(logText);
            }
        }
    }

    /// <summary>
    /// 方法执行后
    /// </summary>
    /// <param name="context"></param>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        ObjectResult rst = context.Result as ObjectResult;
        object? rstValue = rst != null ? rst.Value : null;
        if (rstValue.GetType().Name != typeof(ApiResult<>).Name)
        {
            if (context.Exception != null)
            {
                // 异常处理
                context.ExceptionHandled = true;
                // 如果是用户异常
                context.HttpContext.Response.StatusCode = EnumResponseStatusCode.InternalServerError.Id;
                context.Result = new ObjectResult(new ApiResultError { Message = context.Exception.Message });
            }
            else
            {
                // 无异常
                context.Result = new ObjectResult(new ApiResult<object?> { Data = rstValue, });
            }
        }

        // 记录请求结果日志
        ControllerActionDescriptor desc = context.ActionDescriptor as ControllerActionDescriptor;
        if (desc != null)
        {
            var logText = CreateResponseLogText(
                context.HttpContext.Request.Method,
                desc.ControllerName,
                desc.ActionName,
                rstValue);
            colaLog.Info(logText);
        }
    }
    
    /// <summary>
    /// 创建请求日志文本
    /// </summary>
    /// <param name="method">请求方法</param>
    /// <param name="controllerName">控制器名称</param>
    /// <param name="actionName">方法名称</param>
    /// <param name="actionArgs">方法参数</param>
    /// <returns></returns>
    private string CreateRequestLogText(string method, string controllerName, string actionName, IDictionary<string, object> actionArgs)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"收到请求[{method}]/{controllerName}/{actionName}，参数：");
        if (actionArgs.Count > 0)
        {
            foreach (var p in actionArgs)
            {
                sb.AppendLine($"    " + p.Key + "：" + Newtonsoft.Json.JsonConvert.SerializeObject(p.Value));
            }
        }
        else
        {
            sb.AppendLine("    无");
        }
        return sb.ToString();
    }

    /// <summary>
    /// 创建响应日志文本
    /// </summary>
    /// <param name="method">请求方法</param>
    /// <param name="controllerName">控制器名称</param>
    /// <param name="actionName">方法名称</param>
    /// <param name="result">执行结果</param>
    /// <returns></returns>
    private string CreateResponseLogText(string method, string controllerName, string actionName, object? result)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"完成请求[{method}]/{controllerName}/{actionName}，结果：");
        if (result != null)
        {
            sb.AppendLine("    " + Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }
        else
        {
            sb.AppendLine("    无");
        }
        return sb.ToString();
    }

}