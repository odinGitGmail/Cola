using Cola.EF.Core.Interfaces;
using Cola.EF.SqlSugar.Repository;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using SqlSugar;

namespace Cola.EF.SqlSugar;

public class UnitOfWork:IUnitOfWork
{
    private readonly IColaDbContextFactory _factory;
    private readonly string _configId;
    private SqlSugarScope _db;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(IColaDbContextFactory factory, string configId = "Default")
    {
        _factory = factory;
        _configId = configId;
        _db = _factory.GetDbContext(_configId);
        _db.BeginTran();
    }

    public IColaBaseRepository<TEntity,TId> GetRepository<TEntity,TId>() where TEntity : IEntity<TId>
    {
        var type = typeof(TEntity);
        if (!_repositories.TryGetValue(type, out var repository))
        {
            repository = new ColaBaseRepository<TEntity,TId>(_factory, _configId);
            _repositories.Add(type, repository);
        }
        return (IColaBaseRepository<TEntity,TId>)repository;
    }

    public void Commit()
    {
        try
        {
            _db.CommitTran();
        }
        catch
        {
            Rollback();
            throw;
        }
    }

    public void Rollback()
    {
        _db.RollbackTran();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }
}