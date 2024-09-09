using Cola.Utils.Constants;
using Microsoft.Extensions.Configuration;

namespace Cola.Authen.Jwt;

public class TokenParameter(IConfigurationManager configurationManager)
{
    public string GetIssuer()
    {
        return configurationManager
            .GetSection(SystemConstant.CONSTANT_COLAAUTH_Jwt_VALIDISSUER_SECTION)
            .Get<string>();
    }

    public string GetAudience()
    {
        return configurationManager
            .GetSection(SystemConstant.CONSTANT_COLAAUTH_Jwt_AVALIDAUDIENCE_SECTION)
            .Get<string>();
    }

    public string GetSecret()
    {
        return configurationManager.GetSection(SystemConstant.CONSTANT_COLAAUTH_SECRET_SECTION)
            .Get<string>();

    }


    public int GetExpiration()
    {
        return configurationManager
            .GetSection(SystemConstant.CONSTANT_COLAAUTH_Jwt_EXPIRATION_SECTION)
            .Get<int>();
    }
}