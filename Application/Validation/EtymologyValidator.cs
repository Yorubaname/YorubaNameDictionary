using FluentValidation;
using YorubaOrganization.Core.Dto.Request;

namespace Application.Validation
{
    public class EtymologyValidator : AbstractValidator<EtymologyDto>
    {
        public EtymologyValidator()
        {
            RuleFor(x => x.Part)
                .NotEmpty().WithMessage("Part is required.");
            RuleFor(x => x.Meaning)
                .NotEmpty().WithMessage("Meaning is required");
        }
    }
}
