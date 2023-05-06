
using DeviceBaseApi.DeviceModule;
using DeviceBaseApi.Utils;
using DeviceBaseApi;

namespace DeviceBaseApiTests;

public class UserFake
{
    public static List<User> Users = new List<User>
    {
        new User
        {
            Sounds = true,
            AccessFailedCount = 0,
            AppMode = AppModeEnum.Dark,
            Created = DateTime.Now,
            Edited = DateTime.Now,
            Devices = new List<Device>(),
            ImageUrl = "https://www.google.com",
            Email = "psp515@wp.pl",
            NormalizedEmail = "PSP515@WP.PL",
            Id = "1",
            Language = LanguageEnum.Polish,
            EmailConfirmed = true,
            Localization = false,
            UserName = "psp515",
            NormalizedUserName = "PSP515",
            LockoutEnabled = false,
            PasswordHash = "Psp515!" , /* Psp515! is password*/
            PhoneNumber="534534345",
            PhoneNumberConfirmed = false,
            PushNotifications = false,
            TwoFactorEnabled = false,
        },

        new User
        {
            Sounds = true,
            AccessFailedCount = 0,
            AppMode = AppModeEnum.Dark,
            Created = DateTime.Now,
            Edited = DateTime.Now,
            Devices = new List<Device>(),
            ImageUrl = "https://www.google.com",
            Email = "lul@wp.pl",
            NormalizedEmail = "LUL@WP.PL",
            Id = "2",
            Language = LanguageEnum.Polish,
            EmailConfirmed = true,
            Localization = false,
            UserName = "lul515",
            NormalizedUserName = "LUL515",
            LockoutEnabled = false,
            PasswordHash = "Psp515!" , /* Psp515! is password*/
            PhoneNumber="534534345",
            PhoneNumberConfirmed = false,
            PushNotifications = false,
            TwoFactorEnabled = false,
        },
    };

    public static List<string> RefreshTokensList = new List<string>();
}
