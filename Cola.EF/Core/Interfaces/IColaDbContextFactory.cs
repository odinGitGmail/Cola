using SqlSugar;

namespace Cola.EF.Core.Interfaces;

public interface IColaDbContextFactory
{
    SqlSugarScope GetDbContext(string configId);
}