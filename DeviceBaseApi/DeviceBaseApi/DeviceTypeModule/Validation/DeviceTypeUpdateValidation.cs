using System;
using DeviceBaseApi.DeviceModule;
using DeviceBaseApi.DeviceModule.DTO;
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

        RuleFor(model => model.MaximalNumberOfUsers)
            .GreaterThan(0)
            .WithMessage("Invalid number of users.");
    }
}

