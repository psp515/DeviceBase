using DeviceBaseApi.UserModule.DTO;
using System.Runtime.ConstrainedExecution;

namespace DeviceBaseApi.UserModule;

public static class UserExtensions
{
    public static void UpdateUserSettings(this User user, UserSettingsDTO dto) 
    {
        user.Edited = DateTime.Now;

        user.AppMode = dto.AppMode; 
        user.Language = dto.Language;
        user.Sounds = dto.Sounds;
        user.PushNotifications = dto.PushNotifications;
        user.PhoneNumber = dto.PhoneNumber;
        user.Localization = dto.Localization;
        user.ImageUrl = dto.ImageUrl;
    }

    public static UserDTO ToUserDTO(this User user)
    {
        return new UserDTO(
            user.AppMode, 
            user.Language, 
            user.ImageUrl, 
            user.UserName, 
            user.PhoneNumber, 
            user.Email, 
            user.TwoFactorEnabled, 
            user.Sounds,
            user.PushNotifications,
            user.Localization,
            user.Edited,
            user.Created);
    }
}
