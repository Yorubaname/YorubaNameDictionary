using Core.Dto.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
