using System;
using System.Text;
using Cola.Models.Core.Models.ColaAuthen;
using Cola.Utils;
using Cola.Utils.Enums;
using Microsoft.AspNetCore.Mvc;
using NetTaste;

namespace Cola.Models.Core.Models;

/// <summary>
///     方法返回类型.
/// </summary>
/// <typeparam name="T">Data 泛型类型.</typeparam>
public class ApiResult<T>
{
    /// <summary>
    /// data
    /// </summary>
    public T? Data { get; set; }
    
    /// <summary>
    /// success is true,otherwise false.
    /// </summary>
    public bool Success { get; set; } = true;
    /// <summary>
    /// message.
    /// </summary>
    public string Message { get; set; } = "";
    /// <summary>
    /// success is 0.
    /// </summary>
    public string ErrorCode { get; set; } = "0";
    /// <summary>
    /// api request path.
    /// </summary>
    public string? RequestPath { get; set; }
    /// <summary>
    /// Token
    /// </summary>
    public TokenModel? Token { get; set; }
}