using DeviceBaseApi.DeviceModule.DTO;
using FluentValidation;

namespace DeviceBaseApi.DeviceModule.Validation;

public class DeviceUpdateValidation : AbstractValidator<DeviceUpdateDTO>
{
    public DeviceUpdateValidation()
    {
        RuleFor(model => model.DeviceName).NotEmpty().Length(3, 30);
        RuleFor(model => model.DevicePlacing).MaximumLength(30);
        RuleFor(model => model.Description).MaximumLength(400);
    }
}
