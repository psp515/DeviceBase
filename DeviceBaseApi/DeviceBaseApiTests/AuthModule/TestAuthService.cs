using DeviceBaseApi;
using DeviceBaseApi.AuthModule;
using DeviceBaseApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.Utils;

namespace DeviceBaseApiTests.AuthModule;

public class TestAuthService : UserFake, IAuthService
{
    public async Task FailLogin(string email)
    {
        var user = Users.SingleOrDefault(x => x.Email == email);

        if (user == null)
            return;

        user.AccessFailedCount += 1;
    }

    public async Task<ServiceResult> Login(string email, string password)
    {
        var user = Users.SingleOrDefault(u => u.Email == email);

        if (user == null)
            return new ServiceResult(false, "User not found.");

        if (user.PasswordHash != password)
            return new ServiceResult(false, "Bad login or password");


        var tokens = new LoginResponseDTO
        {
            AccessToken = await GenerateAccessToken(user),
            RefreshToken = GenerateRefreshToken()
        };

        return new ServiceResult(true, Value:tokens);
    }

    public async Task<ServiceResult> RefreshTokens(string refreshToken, string guid)
    {
        var token = RefreshTokensList.SingleOrDefault(x => x == refreshToken);

        if (token == null)
            return new ServiceResult(false, "Token not found");

        RefreshTokensList.Remove(token);

        var user = Users.SingleOrDefault(u => u.Id == guid);

        if (user == null)
            return new ServiceResult(false, "User not found.");

        var tokens = new LoginResponseDTO
        {
            AccessToken = await GenerateAccessToken(user),
            RefreshToken = GenerateRefreshToken()
        };

        return new ServiceResult(true, Value: tokens);
    }

    public async Task<ServiceResult> Register(User user, string password)
    {
        if (Users.Any(x => x.Email == user.Email))
            return new ServiceResult(false, "User exists");

        user.PasswordHash = password;
        Users.Add(user);

        return new ServiceResult(true);
    }

    public async Task<bool> UserExists(string email)
    {
        return Users.Any(x => x.Email == email);
    }

    public async Task<string> GenerateAccessToken(User user)
    {
        var tokenId = Guid.NewGuid();

        var role = "psp515@wp.pl" == user.Email ? ApplicationRoles.Admin : ApplicationRoles.AuthorizedUser;

        var claims = new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, tokenId.ToString()),
            new Claim(ClaimTypes.Role, role) /* For testing purpose. */
        });
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.Now.AddMinutes(20),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("THIS IS USED TO SIGN AND VERIFY JWT TOKENS, REPLACE IT WITH YOUR OWN SECRET")), SecurityAlgorithms.HmacSha256Signature)
        };

        return CreateToken(tokenDescriptor);
    }

    public string GenerateRefreshToken()
    {
        var tokenId = Guid.NewGuid();

        RefreshTokensList.Add(tokenId.ToString());

        return tokenId.ToString();
    }

    private string CreateToken(SecurityTokenDescriptor tokenDescriptor)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);

        return $"Bearer " + tokenHandler.WriteToken(securityToken);
    }
}
