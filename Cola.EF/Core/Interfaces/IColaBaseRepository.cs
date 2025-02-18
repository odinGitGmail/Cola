using System.Linq.Expressions;
using Cola.EF.SqlSugar;
using Cola.Models.Core.Models.ColaEF;
using SqlSugar;

namespace Cola.EF.Core.Interfaces;

public interface IColaBaseRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
{
    #region Find

    TEntity? GetSingleOrDefault(TKey id);
    
    TEntity? GetSingleOrDefault(Expression<Func<TEntity, bool>> whereExpression);

    List<TEntity> GetEntities(
        Expression<Func<TEntity, bool>>? whereExpression,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null);

    int GetCount(
        Expression<Func<TEntity, bool>>? whereExpression,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null);

    TEntity? GetFirstOrDefaultEntity(
        List<OrderExpression<TEntity>> orderExpressions,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null);

    /// <summary>
    /// 通用的查询方法，支持单表查询和多表联查
    /// </summary>
    /// <typeparam name="TResult">返回的 DTO 类型</typeparam>
    /// <param name="selectExpression">选择字段表达式</param>
    /// <param name="joinExpression">联表表达式（可为空）</param>
    /// <param name="whereExpression">查询条件表达式（可为空）</param>
    /// <param name="whereIfExpressions">动态条件表达式集合（可为空）</param>
    /// <param name="orderExpressions">多排序表达式集合（可为空）</param>
    /// <returns></returns>
    List<TResult> Query<TResult>(
        Expression<Func<TEntity, TResult>> selectExpression,
        Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>>? joinExpression = null,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        List<OrderExpression<TEntity>>? orderExpressions = null);
    
    public PageQueryResponse<TResult> QueryPageing<TResult>(
        PageQueryResponse<TResult> pageQueryResponse,
        Expression<Func<TEntity, TResult>> selectExpression,
        Expression<Func<TEntity,TKey>> primaryKeyExpression,
        Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>>? joinExpression=null,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        List<OrderExpression<TEntity>>? orderExpressions = null,
        int pageIndex = 0,
        int pageSize = 10);
    
    #endregion
    
    #region FindAsync

    Task<int> GetCountAsync(
        Expression<Func<TEntity, bool>>? whereExpression,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null);

    Task<TEntity>? GetFirstEntityAsync(
        List<OrderExpression<TEntity>> orderExpressions,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null);
    
    Task<PageQueryResponse<TResult>> QueryPageingAsync<TResult>(
        PageQueryResponse<TResult> pageQueryResponse,
        Expression<Func<TEntity, TResult>> selectExpression,
        Expression<Func<TEntity,TKey>> primaryKeyExpression,
        Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>>? joinExpression=null,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        List<OrderExpression<TEntity>>? orderExpressions = null,
        int pageIndex = 0,
        int pageSize = 10);

    #endregion

    //
    // Task<PageResult<TResult>> QueryPagedAsync<TRequest, TResult>(PageRequest<TRequest> request)
    //     where TRequest : class, new() where TResult : class, new();
}