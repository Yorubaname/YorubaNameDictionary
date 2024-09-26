using Application.Exceptions;
using Core.Repositories;
using YorubaOrganization.Core.Entities;

namespace Application.Services
{
    public class GeoLocationsService
    {
        private readonly IGeoLocationsRepository _geoLocationsRepository;

        public GeoLocationsService(IGeoLocationsRepository geoLocationsRepository)
        {
            _geoLocationsRepository = geoLocationsRepository;
        }
        public async Task<List<GeoLocation>> GetAll()
        {
            return await _geoLocationsRepository.GetAll();
        }
        public async Task Create(GeoLocation geoLocation)
        {
            var match = await _geoLocationsRepository.FindByPlace(geoLocation.Place);
            if (match != null)
            {
                throw new ClientException("This location already exists.");
            }

            await _geoLocationsRepository.Create(new GeoLocation
            {
                Place = geoLocation.Place.Trim().ToUpper(),
                Region = geoLocation.Region.Trim().ToUpper(),
                CreatedBy = geoLocation.CreatedBy,
            });
        }

        public async Task Delete(string id, string place)
        {
            if (string.IsNullOrWhiteSpace(place) || string.IsNullOrWhiteSpace(id))
            {
                throw new ClientException("One or more input parameters are not valid.");
            }

            var deleteCount = await _geoLocationsRepository.Delete(id, place);
            if (deleteCount == 0)
            {
                throw new ClientException("No matching records were found to delete.");
            }
        }
    }
}
