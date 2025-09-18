using Words.Core.Dto.Request;
using FluentValidation;

namespace Application.Validation
{
    public class WordValidator : AbstractValidator<WordDto>
    {
        public WordValidator(
            GeoLocationValidator geoLocationValidator,
            EmbeddedVideoValidator embeddedVideoValidator,
            EtymologyValidator etymologyValidator,
            DefinitionValidator definitionValidator)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Word)
                .NotEmpty().WithMessage("Word is required.")
                .Length(2, 50).WithMessage("Word must be 2 to 50 characters.");

            RuleFor(x => x.PartOfSpeech)
                .IsInEnum().WithMessage("Part of Speech is required.");

            RuleForEach(x => x.Etymology)
                .SetValidator(etymologyValidator);

            RuleForEach(x => x.Videos)
                .SetValidator(embeddedVideoValidator);

            RuleForEach(x => x.GeoLocation)
                .SetValidator(geoLocationValidator);

            RuleForEach(x => x.Definitions)
                .SetValidator(definitionValidator);

            RuleFor(x => x.SubmittedBy)
                .NotEmpty().WithMessage("Submitted By is required.");
        }
    }
}