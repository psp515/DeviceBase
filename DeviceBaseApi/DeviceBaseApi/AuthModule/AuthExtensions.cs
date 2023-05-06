using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.Utils;

namespace DeviceBaseApi.AuthModule;

public static class AuthExtensions
{
    public static User CreateUser(this RegisterRequestDTO dto, string defaultImage)
    {
        return new User
        {
            Email = dto.Email,
            NormalizedEmail = dto.Email.ToUpper(),
            UserName = dto.UserName,
            NormalizedUserName = dto.UserName.ToUpper(),
            PhoneNumber = "",


            TwoFactorEnabled = false,
            EmailConfirmed = false,
            AccessFailedCount = 0,
            PhoneNumberConfirmed = false,
            LockoutEnabled = false,

            ImageUrl = defaultImage,
            Sounds = false,
            Localization = false,
            Language = 0,
            PushNotifications = false,
            AppMode = AppModeEnum.Universal,

            Edited = DateTime.Now,
            Created = DateTime.Now
        };
    }
}
