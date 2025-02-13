﻿using FluentValidation;
using YorubaOrganization.Core.Dto.Request;
using YorubaOrganization.Core.Repositories;

namespace Application.Validation
{
    public class GeoLocationValidator : AbstractValidator<CreateGeoLocationDto>
    {
        private readonly IGeoLocationsRepository _geoLocationsRepository;

        public GeoLocationValidator(IGeoLocationsRepository geoLocationsRepository)
        {
            _geoLocationsRepository = geoLocationsRepository;

            RuleFor(x => x.Place)
                .NotEmpty().WithMessage("Place must be provided")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Region)
                        .NotEmpty().WithMessage("Region must be provided")
                        .DependentRules(() =>
                        {
                            RuleFor(x => x)
                                .CustomAsync(async (x, context, cancellation) =>
                                {
                                    var location = await _geoLocationsRepository.FindByPlaceAndRegion(x.Region, x.Place);
                                    if (location == null)
                                    {
                                        context.AddFailure("The specified Region and Place combination is not valid.");
                                    }
                                });
                        });
                });
        }
    }

}
