using DeviceBaseApi.DeviceModule;
using Microsoft.AspNetCore.Identity;

namespace DeviceBaseApi.AuthModule;

public class User : IdentityUser, ICloneable
{
    public List<Device> Devices { get; set; } = new List<Device>();

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
