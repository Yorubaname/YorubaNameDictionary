using Application.Services;
using Microsoft.Extensions.Localization;
using Website.Pages.Shared;
using Website.Resources;
using Website.Services;

namespace Website.Pages
{
    public class IndexModel(IStringLocalizer<Messages> localizer, ApiService apiService) : BasePageModel(localizer)
    {
        public string Title { get; private set; } = string.Empty;
        public int NameCount { get; private set; }
        public string[] LatestSearches { get; private set; } = [];
        public string[] LatestAdditions { get; private set; } = [];
        public string[] MostPopular { get; private set; } = [];
        public List<string> Alphabets { get; private set; } = [];
        public ApiService _apiService = apiService;

        public async Task OnGet()
        {
            var searchActivity = await _apiService.GetRecentStats();
            var indexedNameCount = await _apiService.GetIndexedNameCount();

            NameCount = indexedNameCount.TotalPublishedNames;
            LatestSearches = searchActivity.LatestSearches;
            LatestAdditions = searchActivity.LatestAdditions;
            MostPopular = searchActivity.MostPopular;
            Alphabets = YorubaAlphabetService.YorubaAlphabets;
        }
    }
}
