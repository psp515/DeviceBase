using AutoMapper;
using DeviceBaseApi.AuthModule;
using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.DeviceModule;
using DeviceBaseApi.DeviceModule.DTO;
using DeviceBaseApi.DeviceTypeModule;
using DeviceBaseApi.DeviceTypeModule.DTO;

namespace DeviceBaseApi;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<Device, DeviceCreateDTO>().ReverseMap();

        CreateMap<Device, DeviceUpdateDTO>().ReverseMap();

        CreateMap<DeviceType, DeviceTypeUpdateDTO>().ReverseMap();
        CreateMap<DeviceType, DeviceTypeCreateDTO>().ReverseMap();

    }
}
