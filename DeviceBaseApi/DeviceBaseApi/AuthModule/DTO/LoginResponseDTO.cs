﻿namespace DeviceBaseApi.AuthModule.DTO;

public class LoginResponseDTO
{
    public UserDTO User { get; set; }
    public string Token { get; set; }
}
