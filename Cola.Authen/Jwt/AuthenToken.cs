using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cola.Utils.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NetTaste;

namespace Cola.Authen.Jwt;

public class AuthenToken(IConfiguration configurationManager) : IAuthenToken
{
    public string GenerateToken(Dictionary<string, string> userClaims)
    {
        var tokenParam = new TokenParameter(configurationManager);
        return CreateToken(userClaims, tokenParam.GetExpiration());
    }
    
    public string GenerateRefreshToken(Dictionary<string, string> userClaims)
    {
        var tokenParam = new TokenParameter(configurationManager);
        return CreateToken(userClaims, tokenParam.GetRefreshExpiration());
    }
    
    private string CreateToken(Dictionary<string,string> userClaims,int expiresTime)
    {
        var tokenParam = new TokenParameter(configurationManager);
        // 注意，必须和上面的 JwtBearer 配置一致，且密钥最少16位，太少会报错！
        SecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenParam.GetSecret()));
        // 同样，我们在上面的 JwtBearer 配置中，需要验证的是什么，这里也要生成对应的条件，缺了就会导致认证失败，假如这里发行人改成其他，验证那边就不通过
        var claims = userClaims.Keys.Select(claim => new Claim(claim, userClaims[claim])).ToList();
        if (!userClaims.ContainsKey(JwtRegisteredClaimNames.Jti))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        }
        SecurityToken securityToken = new JwtSecurityToken(
            // 和上面一样，同样遵循 3+2 规则
            issuer: tokenParam.GetIssuer(),        // 发行人
            audience: tokenParam.GetAudience(),        // 订阅人
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256),        // 安全密钥 和 加密算法
            expires: DateTime.Now.AddMinutes(expiresTime),         // 过期时间
            claims: claims        // 添加 Claim（声明主体），添加uid、username、role等都放在这里
        );
        
        return new JwtSecurityTokenHandler().WriteToken(securityToken);    // 返回 Token字符串
    }
    
    private string ValidateRefreshToken(string refreshToken)
    {
        // 验证 Refresh Token 是否有效，并返回用户名
        var tokenParam = new TokenParameter(configurationManager);
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = tokenParam.GetIssuer(),
                ValidAudience = tokenParam.GetAudience(),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenParam.GetSecret()))
            }, out var validatedToken);

            var jwtToken = validatedToken as JwtSecurityToken;
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var username = principal.Identity.Name;
            return username;
        }
        catch
        {
            return null;
        }
    }
}