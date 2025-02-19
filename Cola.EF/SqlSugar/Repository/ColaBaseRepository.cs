using System.Linq.Expressions;
using Cola.EF.Core;
using Cola.EF.Core.Interfaces;
using Cola.Models.Core.Models.ColaEF;
using Org.BouncyCastle.Ocsp;
using SqlSugar;

namespace Cola.EF.SqlSugar.Repository;

public class ColaBaseRepository<TEntity, TKey>(IColaDbContextFactory factory, string configId = "Default")
    : IColaBaseRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>, new()
{
    private readonly SqlSugarScope _db = factory.GetDbContext(configId);

    #region private method

    private ISugarQueryable<TEntity> GetEntitiesToISugarQueryable(
        Expression<Func<TEntity, bool>>? whereExpression,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        string? tableName = null)
    {
        var query = whereExpression == null
            ? (string.IsNullOrEmpty(tableName)
                ?_db.Queryable<TEntity>()
                :_db.Queryable<TEntity>().AS(tableName))
            : (string.IsNullOrEmpty(tableName)
                ?_db.Queryable<TEntity>().Where(whereExpression)
                :_db.Queryable<TEntity>().AS(tableName).Where(whereExpression));
        if (whereIfExpressions != null)
        {
            foreach (var whereIfExpression in whereIfExpressions)
            {
                query.WhereIF(whereIfExpression.IsWhere, whereIfExpression.WhereIf);
            }
        }

        return query;
    }

    private ISugarQueryable<TEntity> GetOrderToISugarQueryable(
        List<OrderExpression<TEntity>> orderExpressions,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        string? tableName = null)
    {
        ISugarQueryable<TEntity> query = GetEntitiesToISugarQueryable(whereExpression, whereIfExpressions, tableName);

        return orderExpressions.Aggregate(
            query,
            (current, orderExpression) =>
                current.OrderBy(orderExpression.Order, orderExpression.OrderType));
    }

    #endregion
    
    #region query
    
    public TEntity? GetSingleOrDefault(TKey id,string? tableName = null)
    {

        return string.IsNullOrEmpty(tableName)
            ? _db.Queryable<TEntity>().InSingle(id)
            : _db.Queryable<TEntity>().AS(tableName).InSingle(id);
    }

    public TEntity? GetSingleOrDefault(Expression<Func<TEntity, bool>> whereExpression,string? tableName = null)
    {
        return string.IsNullOrEmpty(tableName)
            ? _db.Queryable<TEntity>().Single(whereExpression)
            : _db.Queryable<TEntity>().AS(tableName).Single(whereExpression);
    }

    public List<TEntity> GetEntities(
        Expression<Func<TEntity, bool>>? whereExpression,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        string? tableName = null)
    {
        return GetEntitiesToISugarQueryable(whereExpression, whereIfExpressions,tableName).ToList();
    }

    public int GetCount(
        Expression<Func<TEntity, bool>>? whereExpression,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        string? tableName = null)
    {
        return GetEntitiesToISugarQueryable(whereExpression, whereIfExpressions, tableName).Count();
    }

    public TEntity? GetFirstOrDefaultEntity(
        List<OrderExpression<TEntity>> orderExpressions,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        string? tableName = null)
    {
        return GetOrderToISugarQueryable(orderExpressions, whereExpression, whereIfExpressions, tableName).First();
    }


    /// <summary>
    /// 通用的查询方法，支持单表查询和多表联查
    /// </summary>
    /// <typeparam name="TResult">返回的 DTO 类型</typeparam>
    /// <param name="selectExpression">选择字段表达式</param>
    /// <param name="joinExpression">联表表达式（可为空）</param>
    /// <param name="whereExpressions">查询条件表达式集合（可为空）</param>
    /// <param name="whereIfExpressions">动态条件表达式集合（可为空）</param>
    /// <param name="orderExpressions">多排序表达式集合（可为空）</param>
    /// <param name="tableName">查询主表的表名（可为空）</param>
    /// <returns></returns>
    public List<TResult> Query<TResult>(
        Expression<Func<TEntity, TResult>> selectExpression,
        Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>>? joinExpression=null,
        List<Expression<Func<TEntity, bool>>>? whereExpressions = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        List<OrderExpression<TEntity>>? orderExpressions = null,
        string? tableName = null)
    {
        var query = string.IsNullOrEmpty(tableName)
            ? _db.Queryable<TEntity>()
            : _db.Queryable<TEntity>().AS(tableName);

        // 联表
        if (joinExpression != null)
        {
            query = joinExpression(query);
        }

        // 查询条件
        if (whereExpressions != null)
        {
            query = whereExpressions.Aggregate(
                query, 
                (current, whereExpression) => 
                    current.Where(whereExpression));
        }

        if (whereIfExpressions != null)
        {
            query = whereIfExpressions.Aggregate(
                query, 
                (current, whereIfExpression) => 
                    current.WhereIF(whereIfExpression.IsWhere, whereIfExpression.WhereIf));
        }
        
        // 排序条件
        if (orderExpressions != null)
        {
            query = orderExpressions.Aggregate(
                query, 
                (current, orderExpression) => 
                    current.OrderBy(orderExpression.Order, orderExpression.OrderType));
        }

        // 选择字段
        return query.Select(selectExpression).ToList();
    }

    /// <summary>
    /// 通用的查询方法，支持单表查询和多表联查
    /// </summary>
    /// <typeparam name="TResult">返回的 DTO 类型</typeparam>
    /// <param name="pageQueryResponse">分页查询返回对象</param>
    /// <param name="selectExpression">选择字段表达式</param>
    /// <param name="primaryKeyExpression">选择字段表达式</param>
    /// <param name="joinExpression">联表表达式（可为空）</param>
    /// <param name="whereExpressions">查询条件表达式集合（可为空）</param>
    /// <param name="whereIfExpressions">动态条件表达式集合（可为空）</param>
    /// <param name="orderExpressions">多排序表达式集合（可为空）</param>
    /// <param name="tableName">查询主表的表名（可为空）</param>
    /// <param name="pageIndex">查询第几页（可为空,默认第一页）</param>
    /// <param name="pageSize">查询每页大小（可为空,默认一页10条记录）</param>
    /// <returns></returns>
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
        int pageSize = 10)
    {
        var query = string.IsNullOrEmpty(tableName)
            ? _db.Queryable<TEntity>()
            : _db.Queryable<TEntity>().AS(tableName);

        // 联表
        if (joinExpression != null)
        {
            query = joinExpression(query);
        }

        // 查询条件
        if (whereExpressions != null)
        {
            query = whereExpressions.Aggregate(
                query, 
                (current, whereExpression) => 
                    current.Where(whereExpression));
        }

        if (whereIfExpressions != null)
        {
            query = whereIfExpressions.Aggregate(
                query, 
                (current, whereIfExpression) => 
                    current.WhereIF(whereIfExpression.IsWhere, whereIfExpression.WhereIf));
        }
        
        // 排序条件
        if (orderExpressions != null)
        {
            query = orderExpressions.Aggregate(
                query, 
                (current, orderExpression) => 
                    current.OrderBy(orderExpression.Order, orderExpression.OrderType));
        }

        
        var totalNumber = 0;
        var primaryKeys = query
            .Select(primaryKeyExpression)
            .ToPageList(pageQueryResponse.PageNumber,pageQueryResponse.PageSize,ref totalNumber);
        
        pageQueryResponse.TotalNumber = totalNumber;
        
        var pagedQuery =string.IsNullOrEmpty(tableName)
            ? _db.Queryable<TEntity>().Where(p => primaryKeys!.Contains(p.Id))
            : _db.Queryable<TEntity>().AS(tableName).Where(p => primaryKeys!.Contains(p.Id));
        
        // 联表（如果传入联表表达式）
        if (joinExpression != null)
        {
            pagedQuery = joinExpression(pagedQuery);
        }

        // 选择字段（如果传入选择字段表达式）
        pageQueryResponse.Data = pagedQuery.Select(selectExpression).ToList();
        
        // 选择字段
        return pageQueryResponse;
    }
    
    #endregion

    #region queryAsync

    public async Task<int> GetCountAsync(
        Expression<Func<TEntity, bool>>? whereExpression,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        string? tableName = null)
    {
        return await GetEntitiesToISugarQueryable(whereExpression, whereIfExpressions, tableName).CountAsync();
    }

    public async Task<TEntity>? GetFirstEntityAsync(
        List<OrderExpression<TEntity>> orderExpressions,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        string? tableName = null)
    {
        return await GetOrderToISugarQueryable(
            orderExpressions,
            whereExpression,
            whereIfExpressions,
            tableName).FirstAsync();
    }

    /// <summary>
    /// 通用的查询方法，支持单表查询和多表联查
    /// </summary>
    /// <typeparam name="TResult">返回的 DTO 类型</typeparam>
    /// <param name="selectExpression">选择字段表达式</param>
    /// <param name="joinExpression">联表表达式（可为空）</param>
    /// <param name="whereExpression">查询条件表达式（可为空）</param>
    /// <param name="whereIfExpressions">动态条件表达式集合（可为空）</param>
    /// <param name="orderExpressions">多排序表达式集合（可为空）</param>
    /// <param name="tableName">查询主表的表名（可为空）</param>
    /// <returns></returns>
    public async Task<List<TResult>> QueryAsync<TResult>(
        Expression<Func<TEntity, TResult>> selectExpression,
        Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>>? joinExpression=null,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        List<OrderExpression<TEntity>>? orderExpressions = null,
        string? tableName = null)
    {
        var query = string.IsNullOrEmpty(tableName)
            ? _db.Queryable<TEntity>()
            : _db.Queryable<TEntity>().AS(tableName);

        // 联表
        if (joinExpression != null)
        {
            query = joinExpression(query);
        }

        // 查询条件
        if (whereExpression != null)
        {
            query = query.Where(whereExpression);
        }

        if (whereIfExpressions != null)
        {
            foreach (var whereIfExpression in whereIfExpressions)
            {
                query = query.WhereIF(whereIfExpression.IsWhere,whereIfExpression.WhereIf);
            }
        }
        
        // 排序条件
        if (orderExpressions != null)
        {
            foreach (var orderExpression in orderExpressions)
            {
                query = query.OrderBy(orderExpression.Order,orderExpression.OrderType);
            }
        }

        // 选择字段
        return await query.Select(selectExpression).ToListAsync();
    }
    
    
    
    /// <summary>
    /// 通用的查询方法，支持单表查询和多表联查
    /// </summary>
    /// <typeparam name="TResult">返回的 DTO 类型</typeparam>
    /// <param name="pageQueryResponse">分页查询返回对象</param>
    /// <param name="selectExpression">选择字段表达式</param>
    /// <param name="primaryKeyExpression">选择字段表达式</param>
    /// <param name="joinExpression">联表表达式（可为空）</param>
    /// <param name="whereExpression">查询条件表达式（可为空）</param>
    /// <param name="whereIfExpressions">动态条件表达式集合（可为空）</param>
    /// <param name="orderExpressions">多排序表达式集合（可为空）</param>
    /// <param name="tableName">查询主表的表名（可为空）</param>
    /// <param name="pageIndex">查询第几页（可为空,默认第一页）</param>
    /// <param name="pageSize">查询每页大小（可为空,默认一页10条记录）</param>
    /// <returns></returns>
    public async Task<PageQueryResponse<TResult>> QueryPageingAsync<TResult>(
        PageQueryResponse<TResult> pageQueryResponse,
        Expression<Func<TEntity, TResult>> selectExpression,
        Expression<Func<TEntity,TKey>> primaryKeyExpression,
        Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>>? joinExpression=null,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        List<OrderExpression<TEntity>>? orderExpressions = null,
        string? tableName = null,
        int pageIndex = 0,
        int pageSize = 10)
    {
        var query = string.IsNullOrEmpty(tableName)
            ? _db.Queryable<TEntity>()
            : _db.Queryable<TEntity>().AS(tableName);

        // 联表
        if (joinExpression != null)
        {
            query = joinExpression(query);
        }

        // 查询条件
        if (whereExpression != null)
        {
            query = query.Where(whereExpression);
        }

        if (whereIfExpressions != null)
        {
            query = whereIfExpressions.Aggregate(
                query, 
                (current, whereIfExpression) => 
                    current.WhereIF(whereIfExpression.IsWhere, whereIfExpression.WhereIf));
        }
        
        // 排序条件
        if (orderExpressions != null)
        {
            query = orderExpressions.Aggregate(
                query, 
                (current, orderExpression) => 
                    current.OrderBy(orderExpression.Order, orderExpression.OrderType));
        }

        
        var totalNumber = 0;
        var primaryKeys = query
            .Select(primaryKeyExpression)
            .ToPageList(pageQueryResponse.PageNumber,pageQueryResponse.PageSize,ref totalNumber);
        
        pageQueryResponse.TotalNumber = totalNumber;
        
        var pagedQuery =string.IsNullOrEmpty(tableName)
            ? _db.Queryable<TEntity>().Where(p => primaryKeys!.Contains(p.Id))
            : _db.Queryable<TEntity>().AS(tableName).Where(p => primaryKeys!.Contains(p.Id));
        
        // 联表（如果传入联表表达式）
        if (joinExpression != null)
        {
            pagedQuery = joinExpression(pagedQuery);
        }

        // 选择字段（如果传入选择字段表达式）
        pageQueryResponse.Data = await pagedQuery.Select(selectExpression).ToListAsync();
        
        // 选择字段
        return pageQueryResponse;
    }

    #endregion

    #region insert

    public int InsertEntities(List<TEntity> entitys, string? tableName = null)
    {
        return string.IsNullOrEmpty(tableName)
            ? _db.Insertable(entitys).ExecuteCommand()
            : _db.Insertable(entitys).AS(tableName).ExecuteCommand();
    }

    public int FastestBulkCopyEntities(List<TEntity> entitys, string? tableName = null)
    {
        return string.IsNullOrEmpty(tableName)
            ? _db.Fastest<TEntity>().BulkCopy(entitys)
            : _db.Fastest<TEntity>().AS(tableName).BulkCopy(entitys);
    }

    #endregion

    #region edit

    public int UpdateEntities(List<TEntity> entitys, string? tableName = null)
    {
        return string.IsNullOrEmpty(tableName)
            ? _db.Updateable(entitys).ExecuteCommand()
            : _db.Updateable(entitys).AS(tableName).ExecuteCommand();
    }

    public int FastestBulkUpdateEntities(List<TEntity> entitys, string? tableName = null)
    {
        return string.IsNullOrEmpty(tableName)
            ? _db.Fastest<TEntity>().BulkUpdate(entitys)
            : _db.Fastest<TEntity>().AS(tableName).BulkUpdate(entitys);
    }
    
    #endregion
    
    #region delete

    public int DeleteEntity(List<TEntity> entitys, string? tableName = null)
    {
        return string.IsNullOrEmpty(tableName)
            ? _db.Deleteable(entitys).ExecuteCommand()
            : _db.Deleteable(entitys).AS(tableName).ExecuteCommand();
    }

    public int DeleteEntities(List<TEntity> entitys, string? tableName = null)
    {
        return string.IsNullOrEmpty(tableName)
            ? _db.Deleteable(entitys).ExecuteCommand()
            : _db.Deleteable(entitys).AS(tableName).ExecuteCommand();
    }

    #endregion
}