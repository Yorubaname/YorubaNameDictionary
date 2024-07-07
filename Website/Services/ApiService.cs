using Core.Dto.Request;
using Core.Dto.Response;
using Microsoft.Extensions.Options;
using Website.Config;

namespace Website.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<ApiSettings> _apiSettings;

        public ApiService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiSettings = apiSettings;
        }

        private async Task<T> GetApiResponse<T>(string endpoint)
        {
            var url = $"{_apiSettings.Value.BaseUrl}{endpoint}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // TODO: Ensure this results in a decent user experience.
                throw new Exception("Error calling API");
            }

            return await response.Content.ReadFromJsonAsync<T>()
                   ?? throw new Exception("No data returned from API");
        }

        public Task<RecentStats> GetRecentStats()
        {
            return GetApiResponse<RecentStats>("/search/activity/all");
        }

        public Task<SearchMetadataDto> GetIndexedNameCount()
        {
            return GetApiResponse<SearchMetadataDto>("/search/meta");
        }

        public Task<GeoLocationDto[]> GetGeoLocations()
        {
            //TODO: Use caching here since this dataset does not often change.
            return GetApiResponse<GeoLocationDto[]>("/geolocations");
        }
    }
}
