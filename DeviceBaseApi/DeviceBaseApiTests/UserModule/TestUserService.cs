
using DeviceBaseApi;
using DeviceBaseApi.UserModule;

namespace DeviceBaseApiTests.UserModule
{
    public class TestUserService : UserFake, IUserService
    {
        public async Task<User> GetUser(string guid)
        {
            return Users.FirstOrDefault(x => x.Id == guid);
        }

        public async Task<User?> UpdateUser(User user)
        {
            var oldUser = Users.FirstOrDefault(x => x.Id == user.Id);

            if (oldUser == null)
                return null;

            Users.Remove(oldUser);
            Users.Add(user);

            return user;
        }
    }
}
