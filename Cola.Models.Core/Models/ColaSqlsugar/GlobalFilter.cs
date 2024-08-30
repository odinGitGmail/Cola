using System;
using SqlSugar;

namespace Cola.Models.Core.Models.ColaSqlsugar;

public class GlobalQueryFilter
{
    public string? ConfigId { get; set; }
    public Action<QueryFilterProvider>? QueryFilter { get; set; }
}