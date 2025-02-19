using Microsoft.IdentityModel.JsonWebTokens;

namespace Cola.Authen;

public interface IAuthenToken
{
    string GenerateToken(Dictionary<string,string> userClaims);
}