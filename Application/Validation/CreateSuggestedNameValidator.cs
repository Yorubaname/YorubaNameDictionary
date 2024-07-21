using Core.Dto.Request;
using Core.Dto.Response;
using Core.Enums;
using FluentValidation;

namespace Application.Validation
{
    public class CreateSuggestedNameValidator :AbstractValidator<CreateSuggestedNameDto>
    {
        public CreateSuggestedNameValidator(GeoLocationValidator geoLocationValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(u => u.Name).NotEmpty().WithMessage("Name is required")
                .Length(2,40).WithMessage("Name must be 2 to 40 characters");
            RuleFor(u => u.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Please enter a valid email address");
            RuleFor(u => u.Details).NotEmpty().WithMessage("Details is required");
            RuleForEach(u => u.GeoLocation).SetValidator(geoLocationValidator);
        }
    }
}
