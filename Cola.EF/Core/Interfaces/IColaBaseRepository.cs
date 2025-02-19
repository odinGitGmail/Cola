using System.Linq.Expressions;
using Cola.EF.SqlSugar;
using Cola.Models.Core.Models.ColaEF;
using SqlSugar;

namespace Cola.EF.Core.Interfaces;

public interface IColaBaseRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
{
    #region Query

    TEntity? GetSingleOrDefault(TKey id,string? tableName = null);
    
    TEntity? GetSingleOrDefault(Expression<Func<TEntity, bool>> whereExpression,string? tableName = null);

    List<TEntity> GetEntities(
        Expression<Func<TEntity, bool>>? whereExpression,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        string? tableName = null);

    int GetCount(
        Expression<Func<TEntity, bool>>? whereExpression,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        string? tableName = null);

    TEntity? GetFirstOrDefaultEntity(
        List<OrderExpression<TEntity>> orderExpressions,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        string? tableName = null);

    /// <summary>
    /// 通用的查询方法，支持单表查询和多表联查
    /// </summary>
    /// <typeparam name="TResult">返回的 DTO 类型</typeparam>
    /// <param name="selectExpression">选择字段表达式</param>
    /// <param name="joinExpression">联表表达式（可为空）</param>
    /// <param name="whereExpressions">查询条件表达式（可为空）</param>
    /// <param name="whereIfExpressions">动态条件表达式集合（可为空）</param>
    /// <param name="orderExpressions">多排序表达式集合（可为空）</param>
    /// <param name="tableName">查询主表的表名（可为空）</param>
    /// <returns></returns>
    List<TResult> Query<TResult>(
        Expression<Func<TEntity, TResult>> selectExpression,
        Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>>? joinExpression = null,
        List<Expression<Func<TEntity, bool>>>? whereExpressions = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        List<OrderExpression<TEntity>>? orderExpressions = null,
        string? tableName = null);
    
    public PageQueryResponse<TResult> QueryPageing<TResult>(
        PageQueryResponse<TResult> pageQueryResponse,
        Expression<Func<TEntity, TResult>> selectExpression,
        Expression<Func<TEntity,TKey>> primaryKeyExpression,
        Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>>? joinExpression=null,
        List<Expression<Func<TEntity, bool>>>? whereExpressions = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        List<OrderExpression<TEntity>>? orderExpressions = null,
        string? tableName = null,
        int pageIndex = 0,
        int pageSize = 10);
    
    #endregion
    
    #region QueryAsync

    Task<int> GetCountAsync(
        Expression<Func<TEntity, bool>>? whereExpression,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        string? tableName = null);

    Task<TEntity>? GetFirstEntityAsync(
        List<OrderExpression<TEntity>> orderExpressions,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        string? tableName = null);
    
    Task<PageQueryResponse<TResult>> QueryPageingAsync<TResult>(
        PageQueryResponse<TResult> pageQueryResponse,
        Expression<Func<TEntity, TResult>> selectExpression,
        Expression<Func<TEntity,TKey>> primaryKeyExpression,
        Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>>? joinExpression=null,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        List<OrderExpression<TEntity>>? orderExpressions = null,
        string? tableName = null,
        int pageIndex = 0,
        int pageSize = 10);

    #endregion
    
    #region insert
    
    int InsertEntities(List<TEntity> entitys, string? tableName = null);
    
    int FastestBulkCopyEntities(List<TEntity> entitys, string? tableName = null);
    
    #endregion

    #region edit
    
    int UpdateEntities(List<TEntity> entitys, string? tableName = null);
    
    int FastestBulkUpdateEntities(List<TEntity> entitys, string? tableName = null);

    #endregion

    #region delete

    int DeleteEntity(List<TEntity> entitys, string? tableName = null);
    
    int DeleteEntities(List<TEntity> entitys, string? tableName = null);

    #endregion
}