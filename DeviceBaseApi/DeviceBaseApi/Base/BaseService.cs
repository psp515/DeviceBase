using AutoMapper;

namespace DeviceBaseApi.Base;

public abstract class BaseService
{

    protected readonly DataContext _db;
    protected readonly ILogger _logger;
    protected readonly IMapper _mapper;

    public BaseService(DataContext db, ILogger logger, IMapper mapper)
    {
        this._db = db;
        this._logger = logger;
        this._mapper = mapper;
    }
}
