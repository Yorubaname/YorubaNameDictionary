using Application.Services.MultiLanguage;
using Words.Core.Dto.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Words.Website.Pages.Shared;
using Words.Website.Resources;
using Words.Website.Services;
using YorubaOrganization.Application.Services;

namespace Words.Website.Pages
{
    public class SearchResultsModel(
        IStringLocalizer<Messages> localizer,
        ILanguageService languageService,
        ApiService apiService) : BasePageModel(localizer, languageService)
    {
        private readonly ApiService _apiService = apiService;

        [BindProperty(SupportsGet = true)]
        [FromQuery(Name = "q")]
        public string? Query { get; set; } = null;
        public WordEntryDto[] Words { get; set; } = [];
        public List<string> Letters { get; private set; } = [];

        public async Task<IActionResult> OnGet()
        {
            if (Query == null)
            {
                // TODO: Create an event to indicate that this page was accessed without a query parameter.
                return RedirectToPage("Index");
            }

            var searchResult = await _apiService.SearchWordAsync(Query);
            Words = searchResult ?? [];

            if (Words.Length == 1 && IsEqualWithoutAccent(Words[0].Word, Query))
            {
                // TODO: Pass this word to the other page
                return RedirectToPage("SingleEntry", new { wordEntry = Query });
            }

            Letters = YorubaAlphabetService.YorubaAlphabet;
            return Page();
        }

        private static bool IsEqualWithoutAccent(string string1, string string2)
        {
            CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            int result = compareInfo.Compare(string1, string2, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
            return result == 0;
        }
    }
}