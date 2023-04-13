
namespace DeviceBaseApi.Controllers;

public abstract class BaseController
{
    protected readonly ILogger<WeatherForecastController> logger;

    public BaseController(ILogger<WeatherForecastController> logger)
	{
        this.logger = logger;
	}
}

