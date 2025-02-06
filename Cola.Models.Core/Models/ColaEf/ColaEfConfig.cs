using SqlSugar;

namespace Cola.Models.Core.Models.ColaEF;

public class ColaEFConfig
{
    public string ConfigId { get; set; } = "Default";
    public DbType DbType { get; set; }
    public string ConnectionString { get; set; } = null!;
    public bool IsAutoCloseConnection { get; set; } = true;
}