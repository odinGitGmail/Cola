using Cola.EF.Core.Interfaces;
using SqlSugar;

namespace Cola.EF.SqlSugar;

public abstract class ColaEntityBase<TKey> : IEntity<TKey> 
{
    [SugarColumn(IsPrimaryKey = true)]
    public TKey Id { get; set; }

    
    
}