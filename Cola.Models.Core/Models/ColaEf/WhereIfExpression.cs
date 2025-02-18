using System;
using System.Linq.Expressions;

namespace Cola.Models.Core.Models.ColaEF;

public class WhereIfExpression<TEntity>
{
    public Expression<Func<TEntity, bool>>? WhereIf { get; set; }

    public bool IsWhere { get; set; }
}