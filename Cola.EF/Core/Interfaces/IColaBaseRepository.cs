using Cola.Models.Core.Models.ColaEF;

namespace Cola.EF.Core.Interfaces;

public interface IColaBaseRepository<out TEntity, in TId> where TEntity : IEntity<TId>
{ 
    TEntity GetById(TId id);

    // PageResult<TResult> QueryPaged<TRequest, TResult>(PageRequest<TRequest> request)
    //     where TRequest : class, new() where TResult : class, new();
    //
    // Task<PageResult<TResult>> QueryPagedAsync<TRequest, TResult>(PageRequest<TRequest> request)
    //     where TRequest : class, new() where TResult : class, new();
}