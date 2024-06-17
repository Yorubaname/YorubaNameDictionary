using Core.Dto.Request;
using Core.Repositories;
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
        private readonly IGeoLocationsRepository _geoLocationsRepository;
        public bool place;
        public bool region;
        public GeoLocationValidator(IGeoLocationsRepository geoLocationsRepository)
        {
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
        }
    }
}

