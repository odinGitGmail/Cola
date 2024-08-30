using Microsoft.AspNetCore.Mvc.Filters;

namespace Cola.EF.UnitOfWork;

/// <summary>
/// 工作单元依赖接口.
/// </summary>
public interface IColaUnitOfWork
{
    /// <summary>
    /// 开启工作单元处理.
    /// </summary>
    /// <param name="context">ActionExecutingContext.</param>
    void BeginTransaction(ActionExecutingContext context);

    /// <summary>
    /// 提交工作单元处理.
    /// </summary>
    /// <param name="resultContext">ActionExecutedContext.</param>
    void CommitTransaction(ActionExecutedContext resultContext);

    /// <summary>
    /// 回滚工作单元处理.
    /// </summary>
    /// <param name="resultContext">ActionExecutedContext.</param>
    void RollbackTransaction(ActionExecutedContext resultContext);

    /// <summary>
    /// 执行完毕（无论成功失败）.
    /// </summary>
    /// <param name="context">ActionExecutingContext.</param>
    /// <param name="resultContext">ActionExecutedContext.</param>
    void OnCompleted(ActionExecutingContext context, ActionExecutedContext resultContext);
}