using System.Linq.Expressions;
using SqlSugar;

namespace Cola.EF.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ISqlSugarClient DbContext { get; }
    IColaBaseRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class, IEntity<TKey>;

    Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>> LeftJoin<TEntity, TJoinEntity>(
        Expression<Func<TEntity, TJoinEntity, bool>> joinExpressio);

    Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>> RightJoin<TEntity, TJoinEntity>(
        Expression<Func<TEntity, TJoinEntity, bool>> joinExpressio);

    Func<ISugarQueryable<TEntity>, ISugarQueryable<TEntity>> InnerJoin<TEntity, TJoinEntity>(
        Expression<Func<TEntity, TJoinEntity, bool>> joinExpressio);

    Expression<Func<TEntity, bool>> WhereExpression<TEntity>(Expression<Func<TEntity, bool>> condation);

    Expression<Func<TEntity, TResult>> SelectExpression<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selectExpression);

    Expression<Func<TEntity, TKey>> PrimaryKeyExpression<TEntity, TKey>(Expression<Func<TEntity, TKey>> condation);

    Expression<Func<TEntity, bool>> QueryPrimaryKeyExpression<TEntity>(Expression<Func<TEntity, bool>> condation);
    
    // 事务相关方法
    void BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();
}