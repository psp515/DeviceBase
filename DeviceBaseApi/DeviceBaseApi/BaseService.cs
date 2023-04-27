using AutoMapper;

namespace DeviceBaseApi;

public abstract class BaseService
{
    private readonly DataContext _db;
    public DataContext db => _db;

    public BaseService(DataContext db)
    {
        this._db = db;
    }
}
