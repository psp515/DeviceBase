namespace DeviceBaseApi.UserModule;

public class UserService : BaseService, IUserService
{
    public UserService(DataContext db) : base(db)
    {
    }

    public async Task<User> GetUser(string guid)
    {
        var foundItem = await db.AppUsers.FindAsync(guid);
        return foundItem;
    }

    public async Task<User> UpdateUser(User user)
    {
        var newItem = db.AppUsers.Update(user).Entity;
        await db.SaveChangesAsync();
        return newItem;
    }
}
