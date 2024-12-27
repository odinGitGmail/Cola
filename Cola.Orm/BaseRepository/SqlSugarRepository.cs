using System.Linq.Expressions;
using Cola.Exception;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace Cola.Orm.BaseRepository;

public class SqlSugarRepository(ISqlSugarClient db) : ISqlSugarRepository
{
    public ISqlSugarClient _db { get; set; } = db;

    public bool Insert<T>(T entity) where T : class, new()
    {
        return _db.Insertable(entity).ExecuteCommand() > 0;
    }

    public bool InsertRange<T>(List<T> entities) where T : class, new()
    {
        return _db.Insertable(entities.ToArray()).ExecuteCommand() > 0;
    }

    public int InsertBulkCopy<T>(List<T> entities) where T : class, new()
    {
        return _db.Fastest<T>().BulkCopy(entities);
    }

    public bool Update<T>(T entity) where T : class, new()
    {
        return _db.Updateable(entity).ExecuteCommand() > 0;
    }

    public bool Update<T>(Expression<Func<T, bool>> whereExpression, Expression<Func<T, T>> updateExpression) where T : class, new()
    {
        return _db.Updateable<T>().SetColumns(updateExpression).Where(whereExpression).ExecuteCommand() > 0;
    }

    public bool Delete<T>(T entity) where T : class, new()
    {
        return _db.Deleteable(entity).ExecuteCommand() > 0;
    }

    public bool Delete<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
    {
        return _db.Deleteable<T>().Where(whereExpression).ExecuteCommand() > 0;
    }

    public T GetById<T>(object id) where T : class, new()
    {
        return _db.Queryable<T>().InSingle(id);
    }

    public List<T> GetAll<T>() where T : class, new()
    {
        return _db.Queryable<T>().ToList();
    }

    public List<T> GetList<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
    {
        return _db.Queryable<T>().Where(whereExpression).ToList();
    }

    public List<T> GetPageList<T>(Expression<Func<T, bool>> whereExpression, PageModel pageModel) where T : class, new()
    {
        var totalCount = 0;
        var result = _db.Queryable<T>().Where(whereExpression).ToPageList(pageModel.PageIndex, pageModel.PageSize, ref totalCount);
        pageModel.TotalCount = totalCount;
        return result;
    }

    public bool Trans(Action<ISqlSugarClient> dbTrans)
    {
        try
        {
            _db.Ado.BeginTran();

            dbTrans(_db);
 
            _db.Ado.CommitTran();
            return true;
        }
        catch (System.Exception ex)
        {
            _db.Ado.RollbackTran();
            throw;
        }
    }
}