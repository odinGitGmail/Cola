using System;
using SqlSugar;

namespace Cola.Models.Core.Models.ColaSqlsugar;

public class AopOnErrorModel
{
    public string? ConfigId { get; set; }
    public Action<SqlSugarException>? AopOnError { get; set; }
}