using DeviceBaseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DeviceBaseApi.DeviceModule;

public class DeviceService : BaseService, IDeviceService
{
    public DeviceService(DataContext db) : base(db)
    {

    }

    public async Task<Device> CreateAsync(Device item)
    {
        var newItem = await db.Devices.AddAsync(item);
        await db.SaveChangesAsync();
        return newItem.Entity;
    }

    public async Task<Device> GetAsync(int id)
    {
        var foundItem = await db.Devices.FindAsync(id);
        return foundItem;
    }

    public async Task<IEnumerable<Device>> GetAllAsync()
    {
        var itemCollection = await db.Devices.ToListAsync();
        return itemCollection;
    }

    public async Task<Device> UpdateAsync(Device item)
    {
        var newItem = db.Devices.Update(item).Entity;
        await db.SaveChangesAsync();
        return newItem;
    }

    public async Task<IEnumerable<Device>> GetUserItemsAsync(string guid)
    {
        var user = await db.AppUsers
            .Include(x => x.Devices)
            .SingleOrDefaultAsync(x => x.Id == guid);

        if (user == null)
            return null;

        var devices = user.Devices.Select(x => { x.Users = null; return x; });

        return devices;
    }

    public async Task<ServiceResult> ConnectDevice(int deviceId, string userId)
    {
        var device = await db.Devices
            .Include(x => x.DeviceType)
            .Include(x => x.Users)
            .FirstAsync(x => x.Id == deviceId);

        if (device == null)
            return new ServiceResult(false, "Device not found.");

        if (device.DeviceType.MaximalNumberOfUsers < device.Users.Count)
            return new ServiceResult(false, "Cannot connect new user.");

        if (string.IsNullOrEmpty(device.OwnerId))
            return new ServiceResult(false, "Device doesn't have a owner.");

        if (device.Users.Any(x => x.Id == userId))
            return new ServiceResult(false, "User already connected.");

        if (!device.NewConnectionsPermitted)
            return new ServiceResult(false, "New connections not perrmited for this device.");

        var user = await db.Users
            .Include(x => x.Devices)
            .FirstAsync(x => x.Id == userId);

        user.Devices.Add(device);
        await db.SaveChangesAsync();

        return new ServiceResult(true);
    }

    public async Task<ServiceResult> DisconnectDevice(int deviceId, string userId)
    {
        var device = await db.Devices
            .Include(x => x.Users)
            .FirstAsync(x => x.Id == deviceId);

        if (device == null)
            return new ServiceResult(false, "Device not found.");

        if (!device.Users.Any(x => x.Id == userId))
            return new ServiceResult(false, "User not connected.");

        var user = await db.Users.FirstAsync(x => x.Id == userId);

        device.Users.Remove(user);
        await db.SaveChangesAsync();

        return new ServiceResult(true);
    }

    public async Task<bool> IsUserConnected(string guid, int id)
    {
        var user = await db.AppUsers.Include(x => x.Devices).FirstAsync(x => x.Id == guid);
        return user.Devices.Any(x => x.Id == id);
    }

    public async Task<ServiceResult> ConnectOwner(int deviceId, string userId, string secret)
    {
        var device = await db.Devices
           .Include(x => x.DeviceType)
           .Include(x => x.Users)
           .FirstAsync(x => x.Id == deviceId);

        if (device == null)
            return new ServiceResult(false, "Device not found.");

        if (!string.IsNullOrEmpty(device.OwnerId))
            return new ServiceResult(false, "Device has owner.");

        if (device.DeviceSecret != secret)
            return new ServiceResult(false, "Invalid secret.");

        var user = await db.Users
            .Include(x => x.Devices)
            .FirstAsync(x => x.Id == userId);

        device.OwnerId = userId;
        user.Devices.Add(device);

        await db.SaveChangesAsync();

        return new ServiceResult(true);
    }

    public async Task<ServiceResult> DisconnectOwner(int deviceId, string userId)
    {
        var device = await db.Devices
            .Include(x => x.Users)
            .FirstAsync(x => x.Id == deviceId);

        if (device == null)
            return new ServiceResult(false, "Device not found.");

        if (device.OwnerId != userId)
            return new ServiceResult(false, "User is not owner.");

        var user = await db.Users.FirstAsync(x => x.Id == userId);

        device.Users.Clear();
        device.OwnerId = "";

        await db.SaveChangesAsync();

        return new ServiceResult(true);
    }

    public async Task<ServiceResult> DeviceConnectionsPolicy(int deviceId, string userId, bool newPolicy)
    {
        var device = await db.Devices
            .Include(x => x.Users)
            .FirstAsync(x => x.Id == deviceId);

        if (device == null)
            return new ServiceResult(false, "Device not found.");

        if (device.OwnerId != userId)
            return new ServiceResult(false, "User is not owner of the device.");

        device.NewConnectionsPermitted = newPolicy;
        await db.SaveChangesAsync();

        return new ServiceResult(true);
    }

    public async Task<bool> IsDeviceOwner(string guid, int id)
    {
        var device = await db.Devices.FirstAsync(x => x.Id == id);
        return device.OwnerId == guid;
    }

    public async Task<bool> DisconnectUser(string guid, int id)
    {
        var device = await db.Devices
             .Include(x => x.Users)
             .FirstAsync(x => x.Id == id);

        if (device.OwnerId == guid)
            return false;

        var user = device.Users.FirstOrDefault(x=> x.Id == guid);

        if (user == null)
            return false;

        device.Users.Remove(user);

        await db.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<User>> GetDeviceUsers(int id)
    {
        var device = await db.Devices
            .Include(x => x.Users)
            .FirstAsync(x => x.Id == id);

        return device.Users;
    }
}
