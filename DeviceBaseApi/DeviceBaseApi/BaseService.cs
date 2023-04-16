using AutoMapper;

namespace DeviceBaseApi;

public abstract class BaseService
{
    protected readonly DataContext db;
    protected readonly IMapper mapper;

    public BaseService(DataContext db, IMapper mapper)
    {
        this.db = db;
        this.mapper = mapper;
    }
}
