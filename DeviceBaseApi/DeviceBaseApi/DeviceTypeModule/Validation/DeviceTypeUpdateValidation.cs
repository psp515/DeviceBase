using DeviceBaseApi.DeviceTypeModule.DTO;
using FluentValidation;

namespace DeviceBaseApi.DeviceTypeModule.Validation;

public class DeviceTypeUpdateValidation : AbstractValidator<DeviceTypeUpdateDTO>
{
    public DeviceTypeUpdateValidation()
    {
        RuleFor(model => model.DefaultName)
           .NotEmpty()
           .WithMessage("Invalid default name.");
    }
}

