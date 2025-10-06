using Application.Services.MultiLanguage;
using Microsoft.Extensions.Localization;
using Words.Website.Pages.Shared;
using Words.Website.Resources;
using Words.Website.Services;
using YorubaOrganization.Application.Services;

namespace Words.Website.Pages
{
    public class IndexModel(IStringLocalizer<Messages> localizer, ILanguageService languageService, ApiService apiService) : BasePageModel(localizer, languageService)
    {
        public long WordCount { get; private set; }
        public string[] LatestSearches { get; private set; } = [];
        public string[] LatestAdditions { get; private set; } = [];
        public string[] MostPopular { get; private set; } = [];
        public List<string> Letters { get; private set; } = [];
        public ApiService _apiService = apiService;

        public async Task OnGet()
        {
            var searchActivity = await _apiService.GetRecentStats();
            var indexedWordCount = await _apiService.GetIndexedWordCount();

            WordCount = indexedWordCount?.TotalPublishedWords ?? 0;
            LatestSearches = searchActivity?.LatestSearches ?? [];
            LatestAdditions = searchActivity?.LatestAdditions ?? [];
            MostPopular = searchActivity?.MostPopular ?? [];
            Letters = YorubaAlphabetService.YorubaAlphabet;
        }
    }
}