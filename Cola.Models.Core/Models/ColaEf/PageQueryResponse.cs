using System.Collections.Generic;

namespace Cola.Models.Core.Models.ColaEF;

public class PageQueryResponse<TEntity>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalNumber { get; set; } = 0;
    public List<TEntity> Data { get; set; }
}