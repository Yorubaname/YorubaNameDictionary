using FluentValidation;
using YorubaOrganization.Core.Dto.Request;
using YorubaOrganization.Core.Enums;

namespace Application.Validation
{
    public class CreateUserValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(u => u.Email).EmailAddress();
            RuleFor(u => u.Password).NotEmpty();
            RuleFor(u => u.Username).NotEmpty();

            RuleFor(u => u.Roles)
                .NotEmpty().WithMessage("No role is selected")
                .Must(roles => roles.All(role => !string.IsNullOrWhiteSpace(role) && Enum.IsDefined(typeof(Role), role)))
                .WithMessage("Invalid role selected");
        }
    }
}
