using AutoMapper;
using DeviceBaseApi.AuthModule;
using DeviceBaseApi.AuthModule.DTO;

namespace DeviceBaseApi;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<User, RegisterRequestDTO>().ReverseMap();
    }
}
