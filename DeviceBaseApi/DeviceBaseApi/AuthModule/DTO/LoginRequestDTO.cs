using static Azure.Core.HttpHeader;

namespace DeviceBaseApi.AuthModule.DTO;

public class LoginRequestDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
}
