using AutoMapper;
using DeviceBaseApi.AuthModule;
using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.Models;
using DeviceBaseApi.Models.DTO;

namespace DeviceBaseApi;

public class MappingConfig : Profile
{
    public MappingConfig()
    {

        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<Coupon, CouponCreateDTO>().ReverseMap();
        CreateMap<Coupon, CouponUpdateDTO>().ReverseMap();
        CreateMap<Coupon, CouponDTO>().ReverseMap();    
    }
}
