using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.AuthModule.Others;

namespace DeviceBaseApi.AuthModule;

public interface IAuthService
{
    public Task<bool> UserExists(string username);
    Task<LoginServiceTO> Login(LoginRequestDTO loginRequestDTO);
    Task<string> Register(RegisterRequestDTO requestDTO);
}
