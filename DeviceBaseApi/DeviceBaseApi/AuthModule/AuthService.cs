using AutoMapper;
using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.AuthModule.Others;
using DeviceBaseApi.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DeviceBaseApi.AuthModule;

public class AuthService : BaseService, IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;
    private string secretKey;

    public AuthService(DataContext db, 
                       ILogger logger, 
                       IMapper mapper,
                       UserManager<User> userManager,
                       RoleManager<IdentityRole> roleManager,
                       IConfiguration configuration) : base(db, logger, mapper) 
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        secretKey = _configuration.GetValue<string>("ApiSettings:Secret");
    }

    public async Task<LoginServiceTO> Login(LoginRequestDTO loginRequestDTO)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == loginRequestDTO.Email);

        if (user == null)
            return new LoginServiceTO("User not found.");
        
        bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

        if (!isValid)
            return new LoginServiceTO("Invalid password.");

        var roles = await _userManager.GetRolesAsync(user);
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, (string) user.Username),
                new Claim(ClaimTypes.Role, (string) roles.FirstOrDefault())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        LoginResponseDTO loginResponseDTO = new()
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token)
        };

        return new LoginServiceTO(loginResponseDTO);
    }

    public async Task<string> Register(RegisterRequestDTO requestDTO)
    {
        var user = _mapper.Map<User>(requestDTO);

        user.Created = DateTime.UtcNow;
        user.Edited = DateTime.UtcNow;


        try
        {
            var result = await _userManager.CreateAsync(user, requestDTO.Password);

            if (result.Succeeded)
            {
                if (!(await _roleManager.RoleExistsAsync("admin")))
                {
                    await _roleManager.CreateAsync(new IdentityRole("admin"));
                    await _roleManager.CreateAsync(new IdentityRole("premiumUser"));
                    await _roleManager.CreateAsync(new IdentityRole("user"));
                }

                await _userManager.AddToRoleAsync(user, "admin");

                return null;
            }

            return "Unknown error.";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    public async Task<bool> UserExists(string email)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
        return user != null;
    }
}
