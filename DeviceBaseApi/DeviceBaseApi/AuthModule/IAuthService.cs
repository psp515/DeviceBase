namespace DeviceBaseApi.AuthModule;

public interface IAuthService
{
    Task<bool> UserExists(string email);
    Task<ServiceResult> Login(string email, string password);
    Task<ServiceResult> Register(User user, string password);
    Task<ServiceResult> RefreshTokens(string refreshToken, string guid);
}

