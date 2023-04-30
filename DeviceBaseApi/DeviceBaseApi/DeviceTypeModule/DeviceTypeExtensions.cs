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
			MaximalNumberOfUsers = dto.MaximalNumberOfUsers
		};
	}

    public static void UpdateDeviceType(this DeviceType type, DeviceTypeUpdateDTO dto)
    {
		type.Edited = DateTime.Now;
        type.DefaultName = dto.DefaultName;
    }
}

