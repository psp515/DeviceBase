using DeviceBaseApi.AuthModule.DTO;

namespace DeviceBaseApi.AuthModule.Others;

public class LoginServiceTO
{
    public LoginServiceTO(string error)
    {
        HasErrors = true;
        Error = error;  
        LoginResponse = null;
    }

    public LoginServiceTO(LoginResponseDTO loginResponse)
    {
        HasErrors = false;
        Error = null;
        LoginResponse = loginResponse;
    }

    public bool HasErrors { get; set; }
    public string Error { get; set; }
    public LoginResponseDTO LoginResponse { get; set; }
}
