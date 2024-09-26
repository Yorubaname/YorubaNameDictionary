using YorubaOrganization.Core.Entities;

namespace Core.Repositories
{
    public interface IGeoLocationsRepository
    {
        Task<List<GeoLocation>> GetAll();

        Task<GeoLocation> FindByPlace(string place);
        Task<GeoLocation> FindByPlaceAndRegion(string region, string place);
        Task Create(GeoLocation geoLocation);
        Task<int> Delete(string id, string place);
    }
}
