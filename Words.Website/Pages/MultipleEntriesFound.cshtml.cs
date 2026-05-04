using Application.Services.MultiLanguage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Words.Core.Dto.Response;
using Words.Website.Pages.Shared;
using Words.Website.Resources;
using Words.Website.Services;
using YorubaOrganization.Application.Services;

namespace Words.Website.Pages
{
    public class MultipleEntriesFoundModel(
        IStringLocalizer<Messages> localizer,
        ILanguageService languageService,
        ApiService apiService) : BasePageModel(localizer, languageService)
    {
        private readonly ApiService _apiService = apiService;

        [BindProperty(SupportsGet = true)]
        [FromQuery(Name = "q")]
        public string? Query { get; set; }

        public WordEntryDto[] Matches { get; private set; } = [];
        public List<string> Letters { get; private set; } = [];

        public async Task<IActionResult> OnGet()
        {
            if (string.IsNullOrWhiteSpace(Query))
            {
                return RedirectToPage("Index");
            }

            Matches = await _apiService.GetWordsByTitle(Query) ?? [];
            if (Matches.Length == 0)
            {
                return RedirectToPage("SearchResults", new { q = Query });
            }

            if (Matches.Length == 1)
            {
                return RedirectToPage("SingleEntry", new { wordEntry = Matches[0].Word });
            }

            Letters = YorubaAlphabetService.YorubaAlphabet;
            return Page();
        }
    }
}
