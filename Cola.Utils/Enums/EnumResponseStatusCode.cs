using System.Collections.Concurrent;

namespace Cola.Utils.Enums;

public class EnumResponseStatusCode(int id, string description) : Enumeration<int>(id, description)
{
    /// <summary>
    /// 200 - ok
    /// </summary>
    public static readonly EnumResponseStatusCode Ok = new EnumResponseStatusCode(200, "OK");
    /// <summary>
    /// 201 - Created
    /// </summary>
    public static readonly EnumResponseStatusCode Created = new EnumResponseStatusCode(201, "Created");
    /// <summary>
    /// 204 - No Content
    /// </summary>
    public static readonly EnumResponseStatusCode NoContent = new EnumResponseStatusCode(204, "No Content");
    /// <summary>
    /// 400 - Bad Request
    /// </summary>
    public static readonly EnumResponseStatusCode BadRequest = new EnumResponseStatusCode(400, "Bad Request");
    /// <summary>
    /// 401 - Unauthorized
    /// </summary>
    public static readonly EnumResponseStatusCode Unauthorized = new EnumResponseStatusCode(401, "Unauthorized");
    /// <summary>
    /// 403 - Forbidden
    /// </summary>
    public static readonly EnumResponseStatusCode Forbidden = new EnumResponseStatusCode(403, "Forbidden");
    /// <summary>
    /// 404 - Not Found
    /// </summary>
    public static readonly EnumResponseStatusCode NotFound = new EnumResponseStatusCode(404, "Not Found") ;
    /// <summary>
    /// 500 - Internal Server Error
    /// </summary>
    public static readonly EnumResponseStatusCode InternalServerError = new EnumResponseStatusCode(500, "Internal Server Error");
}