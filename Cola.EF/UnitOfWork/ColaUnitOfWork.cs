using Microsoft.AspNetCore.Mvc.Filters;
using SqlSugar;

namespace Cola.EF.UnitOfWork;

public class ColaUnitOfWork(ISqlSugarClient sqlSugarClient) : IColaUnitOfWork
{
    public void BeginTransaction(ActionExecutingContext context)
    {
        sqlSugarClient.AsTenant().BeginTran();
    }

    public void CommitTransaction(ActionExecutedContext resultContext)
    {
        sqlSugarClient.AsTenant().CommitTran();
    }

    public void OnCompleted(ActionExecutingContext context, ActionExecutedContext resultContext)
    {
        sqlSugarClient.Dispose();
    }

    public void RollbackTransaction(ActionExecutedContext resultContext)
    {
        sqlSugarClient.AsTenant().RollbackTran();
    }
}