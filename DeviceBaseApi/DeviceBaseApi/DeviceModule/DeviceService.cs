using DeviceBaseApi.DeviceTypeModule;
using Microsoft.EntityFrameworkCore;

namespace DeviceBaseApi.DeviceModule;

public class DeviceService : BaseService, IDeviceService
{
    public DeviceService(DataContext db) : base(db)
    {

    }

    public async Task<ICollection<Device>> GetUserItems(string guid)
    {
        var user = await db.AppUsers
            .Include(x => x.Devices)
            .FirstAsync(x => x.Id == guid);

        return user.Devices;
    }
    public async Task<Connection> ConnectDevice(int deviceId, string userId)
    {
        var device = await db.Devices
            .Include(x => x.DeviceType)
            .Include(x => x.Users)
            .FirstAsync(x => x.Id == deviceId);

        if (device == null)
            return new Connection(false, "Device not found.");

        if (device.DeviceType.MaximalNumberOfUsers < device.Users.Count)
            return new Connection(false, "Cannot connect new user.");

        if (device.Users.Any(x=>x.Id == userId))
            return new Connection(false, "User already connected.");

        var user = await db.Users
            .Include(x => x.Devices)
            .FirstAsync(x => x.Id == userId);

        user.Devices.Add(device);
        await db.SaveChangesAsync();

        return new Connection(true);
    }

    public async Task<Connection> DisconnectDevice(int deviceId, string userId)
    {
        var device = await db.Devices
            .Include(x => x.Users)
            .FirstAsync(x => x.Id == deviceId);

        if (device == null)
            return new Connection(false, "Device not found.");

        if (!device.Users.Any(x => x.Id == userId))
            return new Connection(false, "User not connected.");

        var user = await db.Users.FirstAsync(x => x.Id == userId);

        device.Users.Remove(user);
        await db.SaveChangesAsync();

        return new Connection(true);
    }

    public async Task<bool> IsUserConnected(string guid, int id)
    {
        var user = await db.AppUsers.Include(x => x.Devices).FirstAsync(x => x.Id == guid);
        return user.Devices.Any(x=>x.Id == id);
    }


}
