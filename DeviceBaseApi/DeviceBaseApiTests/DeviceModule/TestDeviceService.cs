
using DeviceBaseApi;
using DeviceBaseApi.DeviceModule;
using DeviceBaseApi.Models;
using System.Net.Http.Headers;

namespace DeviceBaseApiTests.DeviceModule;

public class TestDeviceService : IDeviceService
{
    List<Device> Devices = new List<Device>
    {
        new Device{
            Id = 1,
            SerialNumber="1200345",
            Description="This is descriptin",
            DeviceName="SP611",
            DevicePlacing="Kitchen",
            DeviceTypeId = 1,
            OwnerId="1",
            NewConnectionsPermitted=true,
            MqttUrl="https://www.google.com",
            Produced= DateTime.Now.AddDays(-1),
            Created = DateTime.Now,
            Edited = DateTime.Now
        },
        new Device{
            Id = 2,
            SerialNumber="1200345",
            Description="This is descriptin",
            DeviceName="SP611E",
            DevicePlacing="No place",
            OwnerId="1",
            NewConnectionsPermitted=true,
            DeviceTypeId = 1,
            MqttUrl="https://www.google.com",
            Produced= DateTime.Now.AddDays(-3),
            Created = DateTime.Now,
            Edited = DateTime.Now
        }
    };

    List<(int, int)> MaxDevices = new List<(int, int)>
    {
        (1, 1),
        (2, 2)
    };

    List<(string, int)> Connections = new List<(string, int)>
    {
        ("1", 1),
    };

    private int Id = 2;

    public async Task<ServiceResult> ConnectDevice(int deviceId, string userId)
    {
        if (!Devices.Any(x => x.Id == deviceId))
            return await Task.FromResult(new ServiceResult(false, "Device doesn't exist."));

        var tuple = (userId, deviceId);

        if (Connections.Any(x => x == tuple))
            return await Task.FromResult(new ServiceResult(false, "Connection already exists."));

        int maxConnections = MaxDevices.SingleOrDefault(x => x.Item1 == deviceId).Item2;

        int current = Connections.Count( x => x.Item2 == deviceId);

        if (maxConnections < current + 1)
            return await Task.FromResult(new ServiceResult(false, "Cannot connect more devices"));

        Connections.Add(tuple);

        return new ServiceResult(true);
    }

    public Task<ServiceResult> ConnectOwner(int deviceId, string userId, string secret)
    {
        throw new NotImplementedException();
    }

    public Task<Device> CreateAsync(Device item)
    {
        Id++;
        item.Id = Id;
        Devices.Add(item);
        return Task.FromResult(item);
    }

    public Task<ServiceResult> DeviceConnectionsPolicy(int deviceId, string userId, bool newPolicy)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResult> DisconnectDevice(int deviceId, string userId)
    {
        var tuple = (userId, deviceId);

        if (!Connections.Any(x => x == tuple))
            return await Task.FromResult(new ServiceResult(false, "Connection doesn't exists."));

        Connections.Remove(tuple);

        return await Task.FromResult(new ServiceResult(true));
    }

    public async Task<ServiceResult> DisconnectOwner(int deviceId, string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DisconnectUser(string guid, int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Device>> GetAllAsync() => await Task.FromResult(Devices);

    public async Task<Device?> GetAsync(int id)
    {
        var item = Devices.SingleOrDefault(x => x.Id == id);

        if (item == null)
            return null;

        return await Task.FromResult(item);
    }

    public Task<IEnumerable<User>> GetDeviceUsers(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Device>> GetUserItemsAsync(string id)
    {
        List<Device> items = new List<Device>();

        foreach(var(guid, deviceId) in Connections)
            if(guid == id)
                if (!items.Any(x => x.Id == deviceId))
                    items.Add(Devices.SingleOrDefault(x => x.Id == deviceId));

        return items;
    }

    public Task<bool> IsDeviceOwner(string guid, int id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsUserConnected(string guid, int id) => await Task.FromResult(Connections.Any(x => x == (guid, id)));

    public async Task<Device?> UpdateAsync(Device item)
    {
        var oldItem = Devices.SingleOrDefault(x => x.Id == item.Id);

        if (oldItem == null)
            return null;

        Devices.Remove(oldItem);
        Devices.Add(item);

        return await Task.FromResult(item);
    }
}
