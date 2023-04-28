using System;
using DeviceBaseApi.Interfaces;

namespace DeviceBaseApi.DeviceTypeModule;

public interface IDeviceTypeService : IGetAsync<DeviceType>, IUpdateAsync<DeviceType>, ICreateAsync<DeviceType>
{

}

