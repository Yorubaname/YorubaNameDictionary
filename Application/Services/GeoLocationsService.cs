using Core.Entities;
using Core.Repositories;

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
    }
}
