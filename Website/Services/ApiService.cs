﻿using Core.Dto.Response;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Website.Config;
using YorubaOrganization.Core.Dto.Response;

namespace Website.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<ApiSettings> _apiSettings;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<ApiService> _logger;

        public ApiService(
            IHttpClientFactory httpClientFactory,
            IOptions<ApiSettings> apiSettings,
            JsonSerializerOptions jsonSerializerOptions,
            ILogger<ApiService> logger 
            )
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiSettings = apiSettings;
            _jsonSerializerOptions = jsonSerializerOptions;
            _logger = logger;
        }

        private async Task<T?> GetApiResponse<T>(string endpoint)
        {
            var url = $"{_apiSettings.Value.BaseUrl}{endpoint}";
            var response = await _httpClient.GetAsync(url);

            var rawContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("GET '{Url}' Failed with '{Status}'; Response: '{Content}'", endpoint, response.StatusCode, rawContent);
                throw new Exception($"Error calling API.");
            }
            if (string.IsNullOrEmpty(rawContent))
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(rawContent, _jsonSerializerOptions);
        }

        public Task<RecentStats> GetRecentStats()
        {
            return GetApiResponse<RecentStats>("/search/activity/all")!;
        }

        public Task<SearchMetadataDto> GetIndexedNameCount()
        {
            return GetApiResponse<SearchMetadataDto>("/search/meta")!;
        }

        public Task<GeoLocationDto[]> GetGeoLocations()
        {
            //TODO: Use caching here since this dataset does not often change.
            return GetApiResponse<GeoLocationDto[]>("/geolocations")!;
        }

        public Task<NameEntryDto[]> SearchNameAsync(string query)
        {
            return GetApiResponse<NameEntryDto[]>("/search/?q=" + query)!;
        }

        public Task<NameEntryDto?> GetName(string nameEntry)
        {
            return GetApiResponse<NameEntryDto?>($"/search/{nameEntry}");
        }

        public Task<NameEntryDto[]> GetAllNamesByAlphabet(string letter)
        {
            return GetApiResponse<NameEntryDto[]>($"/search/alphabet/{letter}")!;
        }
    }
}
