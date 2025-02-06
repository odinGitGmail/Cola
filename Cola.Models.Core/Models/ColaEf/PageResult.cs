using System;
using System.Collections.Generic;

namespace Cola.Models.Core.Models.ColaEF;

public class PageResult<T>(IEnumerable<T> result, int totalCount, int pageSize)
{
    public IEnumerable<T> Result { get; } = result;
    public int TotalCount { get; } = totalCount;
    public int TotalPages { get; } = (int)Math.Ceiling(totalCount / (double)pageSize);
}