using FluentValidation;
using DeviceBaseApi.Coupons.DTO;

namespace DeviceBaseApi.Coupons.Validations;

public class CouponCreateValidation : AbstractValidator<CouponCreateDTO>
{
    public CouponCreateValidation()
    {
        RuleFor(model => model.Name).NotEmpty();
        RuleFor(model => model.Percent).InclusiveBetween(1, 100);
    }
}
