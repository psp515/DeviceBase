using DeviceBaseApi.UserModule.DTO;
using FluentValidation;
using System.Text.RegularExpressions;

namespace DeviceBaseApi.UserModule.Validation;

public class UserSettingsValidation : AbstractValidator<UserSettingsDTO>
{
    public UserSettingsValidation()
    {

        RuleFor(model => model.PhoneNumber)
            .Must(IsPhoneNumber)
            .WithMessage("Invalid phone format should be +xx-xxx-xxx-xxx.");

        RuleFor(model => model.ImageUrl)
            .Must(IsUrl)
            .WithMessage("Image is not valid url.");

        RuleFor(model => model.Language)
            .IsInEnum()
            .WithMessage("Invalid Language enum value.");

        RuleFor(model => model.AppMode)
            .IsInEnum()
            .WithMessage("Invalid App Mode enum value.");
    }

    private static bool IsPhoneNumber(string number) 
    {
        string pattern = @"^\+?\d{0,2}\-?\d{3}\-?\d{3}\-?\d{3,4}$";
        Regex regex = new Regex(pattern);

        return regex.Match(number).Success;
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
