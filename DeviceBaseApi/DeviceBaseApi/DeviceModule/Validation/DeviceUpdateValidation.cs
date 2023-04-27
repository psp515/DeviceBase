using DeviceBaseApi.DeviceModule.DTO;
using FluentValidation;

namespace DeviceBaseApi.DeviceModule.Validation;

public class DeviceUpdateValidation : AbstractValidator<DeviceUpdateDTO>
{
    public DeviceUpdateValidation()
    {
        RuleFor(model => model.DeviceName)
            .NotEmpty()
            .Length(3, 30)
            .WithMessage("Device name is invalid (3 - 30 characters).");

        RuleFor(model => model.DevicePlacing)
            .MaximumLength(30)
            .WithMessage("Invalid placing - max length 3 characters.");

        RuleFor(model => model.Description)
            .MaximumLength(400)
            .WithMessage("Description is too long (max. 400 characters).");
    }
}
