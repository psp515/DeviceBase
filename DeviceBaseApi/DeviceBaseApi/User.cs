using DeviceBaseApi.DeviceModule;
using Microsoft.AspNetCore.Identity;

namespace DeviceBaseApi;

public class User : IdentityUser
{
    public List<Device> Devices { get; set; } = new List<Device>();

    public AppModeEnum AppMode { get; set; }
    public string Language { get; set; }
    public string ImageUrl { get; set; }
    public bool Sounds { get; set; }
    public bool PushNotifications { get; set; }
    public bool Localization { get; set; }
    public DateTime Edited { get; set; }
    public DateTime Created { get; set; }
}
