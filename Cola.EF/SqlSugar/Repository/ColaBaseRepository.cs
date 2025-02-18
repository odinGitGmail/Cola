using System.Linq.Expressions;
using Cola.EF.Core;
using Cola.EF.Core.Interfaces;
using Cola.Models.Core.Models.ColaEF;
using Org.BouncyCastle.Ocsp;
using SqlSugar;

namespace Cola.EF.SqlSugar.Repository;

public class ColaBaseRepository<TEntity,TKey> : IColaBaseRepository<TEntity,TKey> where TEntity : class, IEntity<TKey>
{
    private readonly SqlSugarScope _db;
    public ColaBaseRepository(IColaDbContextFactory factory, string configId = "Default")
    {
        _db = factory.GetDbContext(configId);
    }
    
    private ISugarQueryable<TEntity> GetEntitiesToISugarQueryable(
        Expression<Func<TEntity, bool>>? whereExpression,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null)
    {
        var query = whereExpression == null
            ? _db.Queryable<TEntity>()
            : _db.Queryable<TEntity>().Where(whereExpression);
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
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null)
    {
        ISugarQueryable<TEntity> query = GetEntitiesToISugarQueryable(whereExpression,whereIfExpressions);
        foreach (var orderExpression in orderExpressions)
        {
            query = query.OrderBy(orderExpression.Order,orderExpression.OrderType);
        }

        return query;
    }
    
    public TEntity? GetSingleOrDefault(TKey id)
    {
        return _db.Queryable<TEntity>().InSingle(id);
    }

    public TEntity? GetSingleOrDefault(Expression<Func<TEntity, bool>> whereExpression)
    {
        return _db.Queryable<TEntity>().Single(whereExpression);
    }

    public List<TEntity> GetEntities(
        Expression<Func<TEntity, bool>>? whereExpression,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null)
    {
        return GetEntitiesToISugarQueryable(whereExpression,whereIfExpressions).ToList();
    }

    public int GetCount(
        Expression<Func<TEntity, bool>>? whereExpression,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null)
    {
        return GetEntitiesToISugarQueryable(whereExpression,whereIfExpressions).Count();
    }

    public TEntity? GetFirstOrDefaultEntity(
        List<OrderExpression<TEntity>> orderExpressions,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null)
    {
        return GetOrderToISugarQueryable(orderExpressions, whereExpression, whereIfExpressions).First();
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
    /// <returns></returns>
    public List<TResult> Query<TResult>(
        Expression<Func<TEntity, TResult>> selectExpression,
        Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>>? joinExpression=null,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        List<OrderExpression<TEntity>>? orderExpressions = null)
    {
        var query = _db.Queryable<TEntity>();

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
        return query.Select(selectExpression).ToList();
    }

    /// <summary>
    /// 通用的查询方法，支持单表查询和多表联查
    /// </summary>
    /// <typeparam name="TResult">返回的 DTO 类型</typeparam>
    /// <param name="selectExpression">选择字段表达式</param>
    /// <param name="primaryKeyExpression">选择字段表达式</param>
    /// <param name="joinExpression">联表表达式（可为空）</param>
    /// <param name="whereExpression">查询条件表达式（可为空）</param>
    /// <param name="whereIfExpressions">动态条件表达式集合（可为空）</param>
    /// <param name="orderExpressions">多排序表达式集合（可为空）</param>
    /// <param name="pageIndex">查询第几页（可为空,默认第一页）</param>
    /// <param name="pageSize">查询每页大小（可为空,默认一页10条记录）</param>
    /// <returns></returns>
    public PageQueryResponse<TResult> QueryPageing<TResult>(
        PageQueryResponse<TResult> pageQueryResponse,
        Expression<Func<TEntity, TResult>> selectExpression,
        Expression<Func<TEntity,TKey>> primaryKeyExpression,
        Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>>? joinExpression=null,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        List<OrderExpression<TEntity>>? orderExpressions = null,
        int pageIndex = 0,
        int pageSize = 10)
    {
        var query = _db.Queryable<TEntity>();

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
        
        var pagedQuery = _db.Queryable<TEntity>()
            .Where(p => primaryKeys!.Contains(p.Id)); 
        
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


    public async Task<int> GetCountAsync(
        Expression<Func<TEntity, bool>>? whereExpression,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null)
    {
        return await GetEntitiesToISugarQueryable(whereExpression,whereIfExpressions).CountAsync();
    }

    public async Task<TEntity>? GetFirstEntityAsync(
        List<OrderExpression<TEntity>> orderExpressions,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null)
    {
        return await GetOrderToISugarQueryable(orderExpressions,whereExpression,whereIfExpressions).FirstAsync();
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
    /// <returns></returns>
    public async Task<List<TResult>> QueryAsync<TResult>(
        Expression<Func<TEntity, TResult>> selectExpression,
        Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>>? joinExpression=null,
        Expression<Func<TEntity, bool>>? whereExpression = null,
        List<WhereIfExpression<TEntity>>? whereIfExpressions = null,
        List<OrderExpression<TEntity>>? orderExpressions = null)
    {
        var query = _db.Queryable<TEntity>();

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
    /// <param name="selectExpression">选择字段表达式</param>
    /// <param name="primaryKeyExpression">选择字段表达式</param>
    /// <param name="joinExpression">联表表达式（可为空）</param>
    /// <param name="whereExpression">查询条件表达式（可为空）</param>
    /// <param name="whereIfExpressions">动态条件表达式集合（可为空）</param>
    /// <param name="orderExpressions">多排序表达式集合（可为空）</param>
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
        int pageIndex = 0,
        int pageSize = 10)
    {
        var query = _db.Queryable<TEntity>();

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
        
        var pagedQuery = _db.Queryable<TEntity>()
            .Where(p => primaryKeys!.Contains(p.Id)); 
        
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
}