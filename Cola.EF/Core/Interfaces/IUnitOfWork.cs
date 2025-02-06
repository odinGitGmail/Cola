namespace Cola.EF.Core.Interfaces;

public interface IUnitOfWork: IDisposable
{
    void Commit();
    void Rollback();
    IColaBaseRepository<TEntity,TId> GetRepository<TEntity,TId>()  where TEntity : IEntity<TId>;
}