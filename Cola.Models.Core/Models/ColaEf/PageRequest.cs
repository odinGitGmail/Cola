using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SqlSugar;

namespace Cola.Models.Core.Models.ColaEF;

public class PageQueryInfo<TEntity>
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public required Expression<Func<TEntity, string>> SelectPrimaryKey { get; set; }
}