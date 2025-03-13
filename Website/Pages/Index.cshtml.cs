using Application.Services.MultiLanguage;
using Microsoft.Extensions.Localization;
using Website.Pages.Shared;
using Website.Resources;
using Website.Services;
using YorubaOrganization.Application.Services;

namespace Website.Pages
{
    public class IndexModel(IStringLocalizer<Messages> localizer, ILanguageService languageService, ApiService apiService) : BasePageModel(localizer, languageService)
    {
        public int NameCount { get; private set; }
        public string[] LatestSearches { get; private set; } = [];
        public string[] LatestAdditions { get; private set; } = [];
        public string[] MostPopular { get; private set; } = [];
        public List<string> Letters { get; private set; } = [];
        public ApiService _apiService = apiService;

        public async Task OnGet()
        {
            // TODO: Parallelize these API calls
            var searchActivity = await _apiService.GetRecentStats();
            var indexedNameCount = await _apiService.GetIndexedNameCount();

            NameCount = indexedNameCount.TotalPublishedNames;
            LatestSearches = searchActivity.LatestSearches;
            LatestAdditions = searchActivity.LatestAdditions;
            MostPopular = searchActivity.MostPopular;
            Letters = YorubaAlphabetService.YorubaAlphabet;
        }
    }
}
