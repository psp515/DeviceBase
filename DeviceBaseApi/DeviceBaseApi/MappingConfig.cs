using AutoMapper;
using DeviceBaseApi.AuthModule.DTO;

namespace DeviceBaseApi;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<User, UserDTO>().ReverseMap();
        //CreateMap<Device, DeviceCreateDTO>().ReverseMap();

        //CreateMap<Device, DeviceUpdateDTO>().ReverseMap();

        //CreateMap<DeviceType, DeviceTypeUpdateDTO>().ReverseMap();
        //CreateMap<DeviceType, DeviceTypeCreateDTO>().ReverseMap();

    }
}
