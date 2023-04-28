using System;
using DeviceBaseApi.DeviceTypeModule.DTO;
using FluentValidation;
using System.Text.Json;

namespace DeviceBaseApi.DeviceTypeModule.Validation;

public class DeviceTypeCreateValidation : AbstractValidator<DeviceTypeCreateDTO>
{
	public DeviceTypeCreateValidation()
	{
        RuleFor(model => model.DefaultName)
           .NotEmpty()
           .WithMessage("Invalid default name.");

        RuleFor(model => model.MaximalNumberOfUsers)
            .GreaterThan(0)
            .WithMessage("Invalid number of users.");

        RuleFor(model => model.EndpointsJson)
            .Must(ContainsElements)
            .WithMessage("Number of endpoints must be at least 1.");
    }

    private static bool ContainsElements(string items) => JsonSerializer.Deserialize<List<string>>(items).Count > 0;
}

