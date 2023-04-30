using DeviceBaseApi.Utils;

namespace DeviceBaseApi.UserModule.DTO;

public record UserDTO(
    AppModeEnum AppMode,
    LanguageEnum Language,
    string ImageUrl,
    string UserName,
    string PhoneNumber,
    string Email,
    bool TwoFactorEnabled,
    bool Sounds,
    bool PushNotifications,
    bool Localization,
    DateTime Edited,
    DateTime Created);
