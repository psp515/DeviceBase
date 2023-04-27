using AutoMapper;
using DeviceBaseApi.AuthModule;
using Microsoft.EntityFrameworkCore;

namespace DeviceBaseApi.DeviceModule;

public class DeviceService : BaseService, IDeviceService
{
    public DeviceService(DataContext db) : base(db)
    {

    }

    public async Task<bool> ConnectDevice(int deviceId, string userId)
    {
        var device = await db.Devices.FirstAsync(x => x.Id == deviceId);
        var user = await db.Users.FirstAsync(x => x.Id == userId);
        user.Devices.Add(device);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DisconnectDevice(int deviceId, string userId)
    {
        var device = await db.Devices.FirstAsync(x => x.Id == deviceId);
        var user = await db.Users.FirstAsync(x => x.Id == userId);
        device.Users.Remove(user);
        await db.SaveChangesAsync();
        return true;
    }
    public async Task<bool> IsUserConnected(string userId, int deviceId)
    {
        var device = await db.Devices.Include(d => d.Users).FirstAsync(device => device.Id == deviceId);
        return device.Users.Any(x=>x.Id == userId);
    }


    public async Task<Device> CreateAsync(Device item)
    {
        var newItem = await db.Devices.AddAsync(item);
        await db.SaveChangesAsync();
        return newItem.Entity;
    }
    public async Task<ICollection<Device>> GetAllAsync()
    {
        return await db.Devices.ToListAsync();
    }
    public async Task<Device> GetAsync(int id)
    {
        return await db.Devices.FirstAsync(item => item.Id == id);
    }
    public async Task<ICollection<Device>> GetByUserIdAsync(string id)
    {
        var user = await db.AppUsers.FirstAsync(x => x.Id == id);
        return user.Devices;
    }

    public async Task<Device> UpdateAsync(Device item)
    {
        var newItem = db.Devices.Update(item);
        await db.SaveChangesAsync();
        return newItem.Entity;
    }


}
