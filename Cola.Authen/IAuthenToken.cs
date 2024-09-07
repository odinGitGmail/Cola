using Microsoft.IdentityModel.JsonWebTokens;

namespace Cola.Authen;

public interface IAuthenToken
{
    string GenerateToekn(Dictionary<string,string> userClaims);
}