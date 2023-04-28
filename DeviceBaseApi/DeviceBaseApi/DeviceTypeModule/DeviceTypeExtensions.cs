using System;
using DeviceBaseApi.DeviceTypeModule.DTO;

namespace DeviceBaseApi.DeviceTypeModule;

public static class DeviceTypeExtensions
{
	public static DeviceType CreateDeviceType(this DeviceTypeCreateDTO dto)
	{
		return new DeviceType
		{
			Edited = DateTime.Now,
			Created = DateTime.Now,

			EndpointsJson = dto.EndpointsJson,
			DefaultName = dto.DefaultName,
			Devices = dto.Devices,
			MaximalNumberOfUsers = dto.MaximalNumberOfUsers
		};
	}

    public static DeviceType UpdateDeviceType(this DeviceTypeUpdateDTO dto, int id)
    {
        return new DeviceType
        {
			Id = id,
            Edited = DateTime.Now,

            DefaultName = dto.DefaultName,
            Devices = dto.Devices,
            MaximalNumberOfUsers = dto.MaximalNumberOfUsers
        };
    }
}

