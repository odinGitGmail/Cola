using System;
using System.Linq.Expressions;
using SqlSugar;

namespace Cola.Models.Core.Models.ColaEF;

public class OrderExpression<TEntity>
{
    public Expression<Func<TEntity, object>>? Order { get; set; }

    public OrderByType OrderType { get; set; }
}