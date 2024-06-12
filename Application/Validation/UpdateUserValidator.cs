using Core.Dto.Request;
using Core.Enums;
using FluentValidation;

namespace Application.Validation
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidator()
        {
            RuleFor(u => u.Roles)
                .Must(roles => roles!.All(role => Enum.IsDefined(typeof(Role), role))).WithMessage("Invalid role selected")
                .When(u => u.Roles != null && u.Roles.Any());
        }
    }
}
