using Core.Dto;
using Core.Dto.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class NameValidator : AbstractValidator<NameDto>
    {

        public NameValidator(GeoLocationValidator geoLocationValidator, EmbeddedVideoValidator embeddedVideoValidator, EtymologyValidator etymologyValidator)
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Meaning)
                .NotEmpty().WithMessage("Meaning is required.");

            RuleForEach(x => x.Etymology)
                .SetValidator(etymologyValidator);

            RuleForEach(x => x.Videos)
                .SetValidator(embeddedVideoValidator);

            RuleForEach(x => x.GeoLocation)
                .SetValidator(geoLocationValidator);

            RuleFor(x => x.FamousPeople)
                .Must(BeAValidCommaSeparatedString).WithMessage("FamousPeople must be a valid comma-separated string.")
                .When(x => x.FamousPeople != null && !string.IsNullOrWhiteSpace(x.FamousPeople.ToString())); ;

            RuleFor(x => x.Syllables)
                .Must(BeAValidHyphenSeparatedString).WithMessage("Syllables must be a valid hyphen-separated string.")
                .When(x => x.Syllables != null && !string.IsNullOrWhiteSpace(x.Syllables.ToString())); ;

            RuleFor(x => x.SubmittedBy)
                .NotEmpty().WithMessage("Submitted By is required.");
        }

        private bool BeAValidCommaSeparatedString(CommaSeparatedString commaSeparatedString)
        {
            if(commaSeparatedString == null)
            {
                return false;
            }

            List<string> items = commaSeparatedString;
            return items.All(item => !string.IsNullOrWhiteSpace(item));
        }

        private bool BeAValidHyphenSeparatedString(HyphenSeparatedString hyphenSeparatedString)
        {
            if (hyphenSeparatedString == null)
                return false;

            List<string> items = hyphenSeparatedString;
            return items.All(item => !string.IsNullOrWhiteSpace(item));
        }

    }
}
