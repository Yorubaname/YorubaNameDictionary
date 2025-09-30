using Microsoft.Extensions.Options;
using System.Text.Json;
using Application.Services.MultiLanguage;
using Words.Core.Dto.Response;
using Words.Website.Config;

namespace Words.Website.Services
{
    public class ApiService(
        IHttpClientFactory httpClientFactory,
        IOptions<ApiSettings> apiSettings,
        JsonSerializerOptions jsonSerializerOptions,
        ILanguageService languageService,
        ILogger<ApiService> logger)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
        private readonly ApiSettings _apiSettings = apiSettings.Value;
        private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;
        private readonly ILanguageService _languageService = languageService;
        private readonly ILogger<ApiService> _logger = logger;

        private async Task<T?> GetApiResponse<T>(string endpoint)
        {
            try
            {
                var url = $"{_apiSettings.BaseUrl}{endpoint}";

                // Create the request message with language header (like the old Website)
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-Tenant", _languageService.CurrentTenant);
                var response = await _httpClient.SendAsync(request);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling API endpoint: {Endpoint}", endpoint);
                throw;
            }
        }

        public Task<WordEntryDto?> GetWord(string wordEntry)
        {
            return GetApiResponse<WordEntryDto?>($"/words/{wordEntry}");
        }

        public Task<WordEntryDto[]?> GetAllWordsByAlphabet(string letter)
        {
            return GetApiResponse<WordEntryDto[]?>($"/words/search/alphabet/{letter}");
        }

        public Task<WordEntryDto[]?> SearchWordAsync(string query)
        {
            return GetApiResponse<WordEntryDto[]?>($"/words/search?q={Uri.EscapeDataString(query)}");
        }

        public Task<WordsMetadataDto?> GetIndexedWordCount()
        {
            return GetApiResponse<WordsMetadataDto>("/words/meta");
        }

        public Task<RecentStatsDto?> GetRecentStats()
        {
            return GetApiResponse<RecentStatsDto>("/words/search/activity");
        }
    }

    public class RecentStatsDto
    {
        public string[] LatestSearches { get; set; } = [];
        public string[] LatestAdditions { get; set; } = [];
        public string[] MostPopular { get; set; } = [];
    }

    public static class YorubaAlphabetService
    {
        public static List<string> YorubaAlphabet =>
            ["A", "B", "D", "E", "Ẹ", "F", "G", "Gb", "H", "I", "J", "K", "L", "M", "N", "O", "Ọ", "P", "R", "S", "Ṣ", "T", "U", "W", "Y"];
    }
}