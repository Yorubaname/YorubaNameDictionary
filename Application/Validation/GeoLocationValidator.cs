using Core.Dto.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class GeoLocationValidator : AbstractValidator<GeoLocationDto>
    {
        public GeoLocationValidator()
        {
           RuleFor(x => x)
                .Must(x => !(string.IsNullOrEmpty(x.Place) || string.IsNullOrEmpty(x.Region)))
                .WithMessage("At least one of Place or Region must be provided");
        }
    }
}
