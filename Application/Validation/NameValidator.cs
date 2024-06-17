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
        public NameValidator(GeoLocationValidator geoLocationValidator)
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Meaning)
                .NotEmpty().WithMessage("Meaning is required.");

            RuleForEach(x => x.Etymology)
                .SetValidator(new EtymologyValidator());

            RuleForEach(x => x.Videos)
                .SetValidator(new EmbeddedVideoValidator());

            RuleForEach(x => x.GeoLocation)
                .SetValidator(geoLocationValidator);

            RuleFor(x => x.FamousPeople)
                .NotNull().WithMessage("FamousPeople is required")
                .Must(BeAValidCommaSeparatedString).WithMessage("FamousPeople must be a valid comma-separated string.");

            RuleFor(x => x.Syllables)
                .NotNull().WithMessage("Syllables is required.")
                .Must(BeAValidHyphenSeparatedString).WithMessage("Syllables must be a valid hyphen-separated string.");

            RuleFor(x => x.SubmittedBy)
                .NotEmpty().WithMessage("Submitted By is required.");
        }

        private bool BeAValidCommaSeparatedString(CommaSeparatedString commaSeparatedString)
        {
            if(commaSeparatedString == null)
            {
                return false;
            }

            var separator = commaSeparatedString.GetSeparatorIn();
            var items = commaSeparatedString.ToString().Split(new[] {separator}, StringSplitOptions.None);
            return items.All(item => !string.IsNullOrWhiteSpace(item));
        }

        private bool BeAValidHyphenSeparatedString(HyphenSeparatedString hyphenSeparatedString)
        {
            if (hyphenSeparatedString == null)
                return false;

            var separator = hyphenSeparatedString.GetSeparatorIn();
            var items = hyphenSeparatedString.ToString().Split(new[] { separator }, StringSplitOptions.None);
            return items.All(item => !string.IsNullOrWhiteSpace(item));
        }

    }
}
