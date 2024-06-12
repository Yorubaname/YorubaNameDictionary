using Core.Dto.Response;
using Core.Enums;
using FluentValidation;

namespace Application.Validation
{
    public class CreateUserValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(u => u.Email).EmailAddress();
            RuleFor(u => u.Password).NotEmpty();
            RuleFor(u => u.Username).NotEmpty();

            // TODO Hafiz: Validate role is in valid list of roles.
            RuleFor(u => u.Roles)
                .NotEmpty().WithMessage("No role is selected")
                .Must(roles => roles.All(role => Enum.IsDefined(typeof(Role), role))).WithMessage("Invalid role selected"); ;
        }
    }
}
