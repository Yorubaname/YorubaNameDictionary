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
    public class MultipleEntriesFoundModel(
        IStringLocalizer<Messages> localizer,
        ILanguageService languageService,
        ApiService apiService) : BasePageModel(localizer, languageService)
    {
        private readonly ApiService _apiService = apiService;

        [BindProperty(SupportsGet = true)]
        [FromQuery(Name = "q")]
        public string? Query { get; set; }

        public NameEntryDto[] Matches { get; private set; } = [];
        public List<string> Letters { get; private set; } = [];

        public async Task<IActionResult> OnGet()
        {
            if (string.IsNullOrWhiteSpace(Query))
            {
                return RedirectToPage("Index");
            }

            Matches = await _apiService.GetNamesByTitle(Query);
            if (Matches.Length == 0)
            {
                return RedirectToPage("SearchResults", new { q = Query });
            }

            if (Matches.Length == 1)
            {
                return RedirectToPage("SingleEntry", new { nameEntry = Matches[0].Name });
            }

            Letters = YorubaAlphabetService.YorubaAlphabet;
            return Page();
        }
    }
}
