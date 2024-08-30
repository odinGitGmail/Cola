using System;
using SqlSugar;

namespace Cola.Models.Core.Models.ColaSqlsugar;

public class AopOnLogExecutingModel
{
    public string? ConfigId { get; set; }
    public Action<string, SugarParameter[]>? AopOnLogExecuting { get; set; }
}