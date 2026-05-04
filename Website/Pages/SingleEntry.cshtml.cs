using Application.Services.MultiLanguage;
using Core.Dto.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Web;
using Website.Pages.Shared;
using Website.Resources;
using Website.Services;
using YorubaOrganization.Application.Services;

namespace Website.Pages
{
    public class SingleEntryModel(
        IStringLocalizer<Messages> localizer,
        ILanguageService languageService,
        ApiService apiService) : BasePageModel(localizer, languageService)
    {
        private readonly ApiService _apiService = apiService;

        public NameEntryDto Name { get; set; } = new NameEntryDto();
        public List<string> Letters { get; private set; } = [];

        public string[] MostPopularNames { get; set; } = [];

        public async Task<IActionResult> OnGet(string nameEntry)
        {
            // TODO: Try to get name from search page first.
            var matches = await _apiService.GetNamesByTitle(nameEntry);

            if (matches.Length == 0)
            {
                return Redirect($"/entries?q={HttpUtility.UrlEncode(nameEntry)}");
            }

            if (matches.Length > 1)
            {
                return RedirectToPage("MultipleEntriesFound", new { q = nameEntry });
            }

            var name = matches[0];

            ViewData["SocialTitle"] = name.Name;
            ViewData["SocialPath"] = $"/entries/{HttpUtility.UrlEncode(name.Name)}";
            ViewData["SocialDescription"] = name.Meaning;

            Name = name;
            Letters = YorubaAlphabetService.YorubaAlphabet;

            var searchActivity = await _apiService.GetRecentStats();
            MostPopularNames = searchActivity.MostPopular;

            return Page();
        }
    }
}
