using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DeviceBaseApi.AuthModule;

public class TokenGenerator : ITokenGenerator
{
    private readonly UserManager<User> _userManager;

    private readonly byte[] _accessTokenSecret;
    private readonly byte[] _refreshTokenSecret;
    private readonly byte _accessTokenMinutes;
    private readonly byte _refreshTokenDays;

    public TokenGenerator(IConfiguration config, UserManager<User> userManager)
    {
        _userManager = userManager;

        _accessTokenSecret = Encoding.ASCII.GetBytes(config.GetValue<string>("Jwt:AccessTokenSecret"));
        _refreshTokenSecret = Encoding.ASCII.GetBytes(config.GetValue<string>("Jwt:RefreshTokenSecret"));
        _accessTokenMinutes = config.GetValue<byte>("Jwt:AccessTokenMinutes");
        _refreshTokenDays = config.GetValue<byte>("Jwt:RefreshTokenDays");
    }

    public async Task<(Guid, string)> GenerateAccessToken(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var tokenId = Guid.NewGuid();
        var claims = new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, tokenId.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
        });
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.Now.AddMinutes(_accessTokenMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_accessTokenSecret), SecurityAlgorithms.HmacSha256Signature)
        };

        return (tokenId, CreateToken(tokenDescriptor));
    }

    public (Guid, string) GenerateRefreshToken()
    {
        var tokenId = Guid.NewGuid();
        var claims = new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, tokenId.ToString())
        });
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.Now.AddDays(_refreshTokenDays),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_refreshTokenSecret), SecurityAlgorithms.HmacSha256Signature)
        };

        return (tokenId, CreateToken(tokenDescriptor));
    }

    private string CreateToken(SecurityTokenDescriptor tokenDescriptor)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);

        return $"Bearer " + tokenHandler.WriteToken(securityToken);
    }
}
