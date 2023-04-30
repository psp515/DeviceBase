namespace DeviceBaseApi.UserModule;

public interface IUserService
{
    Task<User> GetUserSettings(string guid);
    Task<User> UpdateUserSettings(User user);
}
