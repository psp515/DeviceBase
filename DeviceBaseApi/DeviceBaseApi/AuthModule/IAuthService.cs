using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.Models;

namespace DeviceBaseApi.AuthModule;

public interface IAuthService
{
    Task<bool> UserExists(string email);
    Task<InternalTO<LoginResponseDTO>> Login(string email, string password);
    Task<InternalTO<UserDTO>> Register(User request, string password);
}
