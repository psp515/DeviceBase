using DeviceBaseApi.AuthModule.DTO;

namespace DeviceBaseApi.AuthModule;

public interface IAuthService
{
    Task<bool> UserExists(string email);
    Task<InternalTO<LoginResponseDTO>> Login(LoginRequestDTO request);
    Task<InternalTO<UserDTO>> Register(RegisterRequestDTO request);
}
