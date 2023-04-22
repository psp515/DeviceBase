using AutoMapper;
using DeviceBaseApi.AuthModule;
using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.DeviceModule;
using DeviceBaseApi.DeviceModule.DTO;

namespace DeviceBaseApi;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<Device, DeviceCreateDTO>().ReverseMap();
        CreateMap<Device, DeviceUpdateDTO>().ReverseMap();
  

    }
}
