using System.Linq.Expressions;
using Cola.EF.Core.Interfaces;
using Cola.EF.SqlSugar.Repository;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using SqlSugar;

namespace Cola.EF.SqlSugar;

public class UnitOfWork : IUnitOfWork
{
    private readonly IColaDbContextFactory _factory;
    private readonly string _configId;
    private readonly SqlSugarScope _db;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(IColaDbContextFactory factory, string configId = "Default")
    {
        _factory = factory;
        _configId = configId;
        _db = factory.GetDbContext(configId);
        _db.BeginTran();
    }
    
    public ISqlSugarClient DbContext => _db;
    
    public IColaBaseRepository<TEntity,TKey> GetRepository<TEntity,TKey>()  where TEntity : class, IEntity<TKey>
    {
        var type = typeof(TEntity);
        if (!_repositories.TryGetValue(type, out var repository))
        {
            repository = new ColaBaseRepository<TEntity,TKey>(_factory, _configId);
            _repositories.Add(type, repository);
        }
        return (IColaBaseRepository<TEntity,TKey>)repository;
    }

    public Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>> LeftJoin<TEntity, TJoinEntity>(
        Expression<Func<TEntity,TJoinEntity,bool>> joinExpressio)
    {
        return queryable => queryable.LeftJoin(joinExpressio);
    }
    
    public Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>> RightJoin<TEntity, TJoinEntity>(
        Expression<Func<TEntity,TJoinEntity,bool>> joinExpressio)
    {
        return queryable => queryable.RightJoin(joinExpressio);
    }
    
    public Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>> InnerJoin<TEntity, TJoinEntity>(
        Expression<Func<TEntity,TJoinEntity,bool>> joinExpressio)
    {
        return queryable => queryable.InnerJoin(joinExpressio);
    }
    
    public Expression<Func<TEntity, bool>> WhereExpression<TEntity>(Expression<Func<TEntity, bool>> condation)
    {
        return condation;
    }

    public Expression<Func<TEntity, TResult>> SelectExpression<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selectExpression)
    {
        return selectExpression;
    }
    
    public Expression<Func<TEntity, TKey>> PrimaryKeyExpression<TEntity,TKey>(Expression<Func<TEntity, TKey>> condation)
    {
        return condation;
    }
    
    public Expression<Func<TEntity, bool>> QueryPrimaryKeyExpression<TEntity>(Expression<Func<TEntity, bool>> condation)
    {
        return condation;
    }

    public void BeginTransaction()
    {
        _db.BeginTran();
    }

    public void CommitTransaction()
    {
        _db.CommitTran();
    }

    public void RollbackTransaction()
    {
        _db.RollbackTran();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }
}