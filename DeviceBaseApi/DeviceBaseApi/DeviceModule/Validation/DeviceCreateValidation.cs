using DeviceBaseApi.DeviceModule.DTO;
using FluentValidation;

namespace DeviceBaseApi.DeviceModule.Validation;

public class DeviceCreateValidation : AbstractValidator<DeviceCreateDTO>
{
    public DeviceCreateValidation()
    {
        RuleFor(model => model.SerialNumber).NotEmpty();
        RuleFor(model => model.MqttUrl).Must(IsUrl);
        RuleFor(model => model.Produced).LessThan(DateTime.Now);
    }

    private static bool IsUrl(string link)
    {
        if (string.IsNullOrWhiteSpace(link))
            return false;
        
        Uri outUri;
        return Uri.TryCreate(link, UriKind.Absolute, out outUri) && 
            (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
    }
}
