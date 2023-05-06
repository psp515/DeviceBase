using DeviceBaseApi.Utils;

namespace DeviceBaseApi.UserModule.DTO;

public record UserSettingsDTO(
    AppModeEnum AppMode, 
    LanguageEnum Language,
    string ImageUrl, 
    string PhoneNumber,
    bool Sounds, 
    bool PushNotifications,
    bool Localization);

