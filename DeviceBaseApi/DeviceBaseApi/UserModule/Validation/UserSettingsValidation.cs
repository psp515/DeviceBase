using DeviceBaseApi.UserModule.DTO;
using FluentValidation;
using System.Text.RegularExpressions;

namespace DeviceBaseApi.UserModule.Validation;

public class UserSettingsValidation : AbstractValidator<UserSettingsDTO>
{
    public UserSettingsValidation()
    {
        RuleFor(model => model.UserName)
            .NotEmpty();

        RuleFor(model => model.PhoneNumber)
            .Must(IsPhoneNumber);

        RuleFor(model => model.ImageUrl)
            .Must(IsUrl);

        RuleFor(model => model.Language)
            .IsInEnum();

        RuleFor(model => model.AppMode)
            .IsInEnum();
    }

    private static bool IsPhoneNumber(string number) 
    {
        string pattern = @"^\+?\d{0,2}\-?\d{3}\-?\d{3}\-?\d{4}$";
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
