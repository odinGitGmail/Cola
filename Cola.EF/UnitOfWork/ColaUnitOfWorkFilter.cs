using Cola.Log.Core;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Cola.EF.UnitOfWork;

public class ColaUnitOfWorkFilter : IAsyncActionFilter, IOrderedFilter
{
    private readonly IColaLog _colaLog;
    public ColaUnitOfWorkFilter(IColaLog colaLog)
    {
        _colaLog = colaLog;
    }
    /// <summary>
    /// 过滤器排序.
    /// </summary>
    private const int CONSTANT_FILTER_ORDER = 999;

    /// <summary>
    /// 排序属性.
    /// </summary>
    public int Order => CONSTANT_FILTER_ORDER;

    /// <summary>
    /// 拦截请求.
    /// </summary>
    /// <param name="context">动作方法上下文.</param>
    /// <param name="next">中间件委托.</param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 获取动作方法描述器
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
        var method = actionDescriptor!.MethodInfo;

        // 获取请求上下文
        var httpContext = context.HttpContext;

        // 如果没有定义工作单元过滤器，则跳过
        if (!method.IsDefined(typeof(ColaUnitOfWorkAttribute), true))
        {
            // 调用方法
            _ = await next();

            return;
        }

        // 打印工作单元开始消息
        _colaLog.Info($@"{nameof(ColaUnitOfWorkFilter)} Beginning");

        // 解析工作单元服务
        var unitOfWork = httpContext.RequestServices.GetRequiredService<IColaUnitOfWork>();

        // 调用开启事务方法
        unitOfWork.BeginTransaction(context);

        // 获取执行 Action 结果
        var resultContext = await next();

        if (resultContext?.Exception == null)
        {
            // 调用提交事务方法
            unitOfWork.CommitTransaction(resultContext!);
        }
        else
        {
            // 调用回滚事务方法
            unitOfWork.RollbackTransaction(resultContext);
        }

        // 调用执行完毕方法
        if (resultContext != null) unitOfWork.OnCompleted(context, resultContext);

        // 打印工作单元结束消息  
        _colaLog.Info($@"{nameof(ColaUnitOfWorkFilter)} Ending");
    }
}