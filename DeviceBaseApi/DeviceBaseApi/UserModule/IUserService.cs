namespace DeviceBaseApi.UserModule;

public interface IUserService
{
    Task<User> GetUser(string guid);
    Task<User> UpdateUser(User user);

}
