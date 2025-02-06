using System.Collections.Concurrent;
using Cola.EF.Core.Interfaces;
using Cola.Models.Core.Models.ColaEF;
using Microsoft.Extensions.Options;
using SqlSugar;

namespace Cola.EF.SqlSugar.Context;

public class ColaDbContextFactory(IOptionsMonitor<List<ColaEFConfig>> dbConfig) : IColaDbContextFactory
{
    private readonly ConcurrentDictionary<string, SqlSugarScope> _contexts = new();

    public SqlSugarScope GetDbContext(string configId)
    {
        return _contexts.GetOrAdd(configId, id =>
        {
            var config = dbConfig.CurrentValue.FirstOrDefault(c => c.ConfigId == id);
            if (config == null) throw new ArgumentException($"Database configuration {id} not found");
            
            return new SqlSugarScope(new ConnectionConfig
            {
                ConfigId = config.ConfigId,
                DbType = config.DbType,
                ConnectionString = config.ConnectionString,
                IsAutoCloseConnection = config.IsAutoCloseConnection
            });
        });
    }

}