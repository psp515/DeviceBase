using System;

namespace DeviceBaseApi.Services;

public abstract class BaseService
{

	protected readonly DataContext dataContext;
    protected readonly ILogger logger;

    public BaseService(DataContext dataContext, ILogger logger)
	{
		this.dataContext = dataContext;
        this.logger = logger;
    }
}

