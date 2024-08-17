using Core.Dto.Request;
using FluentValidation;
namespace Application.Validation
{
    public class NameValidator : AbstractValidator<NameDto>
    {
        public NameValidator(GeoLocationValidator geoLocationValidator, EmbeddedVideoValidator embeddedVideoValidator, EtymologyValidator etymologyValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(2, 50).WithMessage("Name must be 2 to 50 chracters");

            RuleFor(x => x.Meaning)
                .NotEmpty().WithMessage("Meaning is required.");

            RuleForEach(x => x.Etymology)
                .SetValidator(etymologyValidator);

            RuleForEach(x => x.Videos)
                .SetValidator(embeddedVideoValidator);

            RuleForEach(x => x.GeoLocation)
                .SetValidator(geoLocationValidator);

            RuleFor(x => x.SubmittedBy)
                .NotEmpty().WithMessage("Submitted By is required.");
        }
    }
}
