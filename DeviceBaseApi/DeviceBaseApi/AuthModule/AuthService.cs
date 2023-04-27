using AutoMapper;
using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DeviceBaseApi.AuthModule;

public class AuthService : BaseService, IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private string secretKey;

    public AuthService(DataContext db, 
        IMapper mapper,
        IConfiguration configuration, 
        UserManager<User> userManager, 
        RoleManager<IdentityRole> roleManager) : base(db)
    {
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        secretKey = _configuration.GetValue<string>("ApiSettings:Secret");
    }

    public async Task<bool> UserExists(string email)
    {
        var user = await db.AppUsers.FirstOrDefaultAsync(x => x.Email == email);
        return user != null;
    }

    public async Task<InternalTO<LoginResponseDTO>> Login(string email, string password)
    {
        var user = await db.AppUsers.FirstOrDefaultAsync(x => x.Email == email);

        if (user == null) 
            return new InternalTO<LoginResponseDTO>("User not found.");
        

        bool isValid = await _userManager.CheckPasswordAsync(user, password);

        if (!isValid) 
            return new InternalTO<LoginResponseDTO>("Invalid password or email.");

        var roles = await _userManager.GetRolesAsync(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                new Claim(ClaimTypes.PrimarySid, user.Id),
            }),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        LoginResponseDTO loginResponseDTO = new()
        {
            User = _mapper.Map<UserDTO>(user),
            Token = $"Bearer " + new JwtSecurityTokenHandler().WriteToken(token)
        };

        return new InternalTO<LoginResponseDTO>(loginResponseDTO);
    }

    public async Task<InternalTO<UserDTO>> Register(User newUser, string password)
    { 

        // TODO: Setup Settings for user 

        try
        {
            var result = await _userManager.CreateAsync(newUser, password);

            if (result.Succeeded)
            {
                await ChcekRoles();

                await _userManager.AddToRoleAsync(newUser, ApplicationRoles.Admin);

                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == newUser.Email);

                return new InternalTO<UserDTO>(_mapper.Map<UserDTO>(user));
            }

            return new InternalTO<UserDTO>(result.Errors.FirstOrDefault().Description);
        }
        catch (Exception e)
        {
            return new InternalTO<UserDTO>(e.Message);
        }
    }

    private async Task ChcekRoles() 
    {
        if (!await _roleManager.RoleExistsAsync(ApplicationRoles.Admin))
            await _roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Admin));

        if (!await _roleManager.RoleExistsAsync(ApplicationRoles.AuthorizedUser))
            await _roleManager.CreateAsync(new IdentityRole(ApplicationRoles.AuthorizedUser));
    }
}
