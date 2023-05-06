using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DeviceBaseApi.Utils;

public static class StringExtensions
{
    public static string GetValueFromToken(this string bearerToken, string secretKey, string typeName = JwtRegisteredClaimNames.Sub)
    {
        var token = bearerToken.Substring("Bearer ".Length).Trim();

        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken.Claims.FirstOrDefault(c => c.Type == typeName).Value;
        }
        catch
        {
            return null;
        }
    }

    public static bool IsTokenExpired(this string bearerToken)
    {
        try
        {
            var token = bearerToken.Substring("Bearer ".Length).Trim();
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            return DateTime.Now > jwtToken.ValidTo;
        }
        catch
        {
            return true;
        }
    }
}

