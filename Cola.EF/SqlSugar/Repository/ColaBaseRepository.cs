using Cola.EF.Core.Interfaces;
using SqlSugar;

namespace Cola.EF.SqlSugar.Repository;

public class ColaBaseRepository<TEntity,TId> : IColaBaseRepository< TEntity,TId> where TEntity : IEntity<TId>
{
    private readonly SqlSugarScope _db;
    public ColaBaseRepository(IColaDbContextFactory factory, string configId = "Default")
    {
        _db = factory.GetDbContext(configId);
    }
    
    public TEntity GetById(TId id)
    {
        return _db.Queryable<TEntity>().InSingle(id);
    }
}