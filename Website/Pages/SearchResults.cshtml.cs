using Application.Services.MultiLanguage;
using Core.Dto.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Website.Pages.Shared;
using Website.Resources;
using Website.Services;
using YorubaOrganization.Application.Services;

namespace Website.Pages
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
        public NameEntryDto[] Names { get; set; } = [];
        public List<string> Letters { get; private set; } = [];

        public async Task<IActionResult> OnGet()
        {
            if (string.IsNullOrWhiteSpace(Query))
            {
                // TODO: Create an event to indicate that this page was accessed without a query parameter.
                return RedirectToPage("Index");
            }

            var exactMatches = await _apiService.GetNamesByTitle(Query);
            if (exactMatches.Length == 1)
            {
                // TODO Hafiz: Pass the entire name entry instead of just the name to avoid another API call in the SingleEntry page.
                return RedirectToPage("SingleEntry", new { nameEntry = exactMatches[0].Name });
            }

            if (exactMatches.Length > 1)
            {
                return RedirectToPage("MultipleEntriesFound", new { q = Query });
            }

            Names = await _apiService.SearchNameAsync(Query);

            Letters = YorubaAlphabetService.YorubaAlphabet;
            return Page();
        }
    }
}
