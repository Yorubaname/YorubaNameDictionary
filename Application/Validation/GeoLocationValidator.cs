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
<<<<<<< HEAD
            _geoLocationsRepository = geoLocationsRepository;

            RuleFor(x => x).MustAsync(async (x, cancellation) =>
            {
                var placeExists = !string.IsNullOrEmpty(x.Place)
                                 && await _geoLocationsRepository.FindByPlace(x.Place.ToUpper()) != null;
                var regionExists = !string.IsNullOrEmpty(x.Region)
                                   && await _geoLocationsRepository.FindByRegion(x.Region.ToUpper()) != null;
                place = placeExists;
                region = regionExists;
                if (!string.IsNullOrWhiteSpace(x.Place) && !string.IsNullOrWhiteSpace(x.Region))
                {
                    return placeExists && regionExists;
                }
                return placeExists || regionExists;
            }).WithMessage((x, validationResult) =>
            {
                if (!string.IsNullOrEmpty(x.Place) && !string.IsNullOrEmpty(x.Region))
                {
                    if (!place && !region)
                    {
                        return $"Place '{x.Place}' and Region '{x.Region}' are not valid.";
                    }
                    else if (!place)
                    {
                        return $"Place '{x.Place}' is not valid.";
                    }
                    else if (!region)
                    {
                        return $"Region '{x.Region}' is not valid.";
                    }
                }
                else if (!place && !region)
                {
                    return "At least one of Place or Region must be provided.";
                }
                return "One or more GeoLocations are not valid.";
            });
=======
           RuleFor(x => x)
                .Must(x => !(string.IsNullOrEmpty(x.Place) || string.IsNullOrEmpty(x.Region)))
                .WithMessage("At least one of Place or Region must be provided");
>>>>>>> b5e889cfd7992c57908bab3da26b61d5aaea9a52
        }
    }
}

