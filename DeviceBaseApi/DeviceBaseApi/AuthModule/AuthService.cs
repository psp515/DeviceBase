using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.Models;
using DeviceBaseApi.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace DeviceBaseApi.AuthModule;

public class AuthService : BaseService, IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenGenerator _tokenGenerator;

    private readonly string _refreshTokenSecret;

    public AuthService(DataContext db,
                       ITokenGenerator tokenGenerator,
                       IConfiguration configuration,
                       UserManager<User> userManager,
                       RoleManager<IdentityRole> roleManager) : base(db)
    {
        _tokenGenerator = tokenGenerator;
        _userManager = userManager;
        _roleManager = roleManager;
        _refreshTokenSecret = configuration.GetValue<string>("Jwt:RefreshTokenSecret");
    }

    public async Task<bool> UserExists(string email)
    {
        var user = await db.AppUsers.FirstOrDefaultAsync(x => x.Email == email);
        return user != null;
    }

    public async Task<ServiceResult> Login(string email, string password)
    {
        var user = await db.AppUsers.SingleOrDefaultAsync(x => x.Email == email);

        if (user == null)
            return new ServiceResult(false, "User not found.");

        bool isValid = await _userManager.CheckPasswordAsync(user, password);

        if (!isValid)
            return new ServiceResult(false, "Invalid password or email.");

        await ChcekRoles();

        var result = await CreateTokens(user);

        return result;
    }

    public async Task<ServiceResult> RefreshTokens(string oldRefreshToken, string guid)
    {
        var user = await db.AppUsers.SingleOrDefaultAsync(x => x.Id == guid);

        if (user == null)
            return new ServiceResult(false, "User not found. Logout user.");

        var tokenId = oldRefreshToken.GetValueFromToken(_refreshTokenSecret, JwtRegisteredClaimNames.Jti);

        if (tokenId == null)
            return new ServiceResult(false, "Invalid refresh token. Logout user.");

        var tokenEntity = await db.UserTokens.SingleOrDefaultAsync(x => x.Name == tokenId);

        if (tokenEntity == null)
            return new ServiceResult(false, "Invalid refresh token. Logout user.");

        if (oldRefreshToken.IsTokenExpired())
            return new ServiceResult(false, "Token expired. Logout user.");

        db.UserTokens.Remove(tokenEntity);

        var result = await CreateTokens(user);

        return result;
    }

    private async Task<ServiceResult> CreateTokens(User user) 
    {
        var (accessTokenId, accessToken) = await _tokenGenerator.GenerateAccessToken(user);
        var (refreshTokenId, refreshToken) = _tokenGenerator.GenerateRefreshToken();

        db.UserTokens.Add(new IdentityUserToken<string>
        {
            UserId = user.Id,
            Name = refreshTokenId.ToString(),
            LoginProvider="Default",
            Value = refreshToken,
        });

        await db.SaveChangesAsync();

        LoginResponseDTO loginResponseDTO = new()
        {
            AccessToken =  accessToken,
            RefreshToken = refreshToken
        };

        return new ServiceResult(true, Value: loginResponseDTO);
    }

    public async Task<ServiceResult> Register(User newUser, string password)
    {
        var result = await _userManager.CreateAsync(newUser, password);

        if (result.Succeeded)
        {
            await ChcekRoles();

            await _userManager.AddToRoleAsync(newUser, ApplicationRoles.AuthorizedUser);

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == newUser.Email);

            return new ServiceResult(true, user.Id);
        }

        return new ServiceResult(false, result.Errors.FirstOrDefault().ToString());
    }

    private async Task ChcekRoles()
    {
        if (!await _roleManager.RoleExistsAsync(ApplicationRoles.Admin))
            await _roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Admin));

        if (!await _roleManager.RoleExistsAsync(ApplicationRoles.AuthorizedUser))
            await _roleManager.CreateAsync(new IdentityRole(ApplicationRoles.AuthorizedUser));
    }

    public async Task FailLogin(string email)
    {
        var user = await db.AppUsers.SingleOrDefaultAsync(x => x.Email == email);
        user.AccessFailedCount++;
        await db.SaveChangesAsync();
    }
}
