using System;
using System.Linq.Expressions;

namespace Cola.Models.Core.Models.ColaEF;

public class PageRequest<T>
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Sort { get; set; }
    public Expression<Func<T,bool>>? Filter { get; set; }
}