using DeviceBaseApi.DeviceModule;
using Microsoft.AspNetCore.Identity;

namespace DeviceBaseApi.AuthModule;

public class User : IdentityUser
{
    public List<Device> Devices { get; set; } = new List<Device>();
}
