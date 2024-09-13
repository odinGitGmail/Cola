using Cola.Utils.Enums;

namespace Cola.Models.Core.Enums.Jwt;

public class AuthEnumeration(int id, string description) : Enumeration<int>(id, description)
{
    public static AuthEnumeration Jwt = new(1, nameof(Jwt));
    public static AuthEnumeration Id4 = new(1, nameof(Id4));
}