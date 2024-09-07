namespace Cola.Models.Core.Models.ColaAuthen;

/// <summary>
/// AccessTokenModel
/// </summary>
public class AccessTokenModel
{
    public string? TokenStr { get; set; }
    public int ExpireTime { get; set; } = -1;
}