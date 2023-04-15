using DeviceBaseApi.AuthModule.DTO;
using FluentValidation;

namespace DeviceBaseApi.AuthModule;

public class UserRegisterValidator : AbstractValidator<RegisterRequestDTO>
{
    public UserRegisterValidator()
    {
        RuleFor(model => model.Email).EmailAddress();
        RuleFor(model => model.Username).Length(3, 50);
        // password is hashed;
    }
}
