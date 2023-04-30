namespace DeviceBaseApi.DeviceTypeModule;

public class DeviceTypeService : BaseService, IDeviceTypeService
{
    public DeviceTypeService(DataContext db) : base(db) { }
}

