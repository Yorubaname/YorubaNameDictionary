using Core.Entities;
using Core.Repositories;
using Application.Exceptions;

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
            throw new DuplicateException("YES an execption occured");
            return await _geoLocationsRepository.GetAll();
        }
    }
}
