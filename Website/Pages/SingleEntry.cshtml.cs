using Application.Services;
using Core.Dto.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Web;
using Website.Pages.Shared;
using Website.Resources;
using Website.Services;

namespace Website.Pages
{
    public class SingleEntryModel(
        IStringLocalizer<Messages> localizer,
        ApiService apiService) : BasePageModel(localizer)
    {
        private readonly ApiService _apiService = apiService;

        public NameEntryDto Name { get; set; } = new NameEntryDto();
        public List<string> Letters { get; private set; } = [];

        public string[] MostPopularNames { get; set; } = [];

        public async Task<IActionResult> OnGet(string nameEntry)
        {
            // TODO: Try to get name from search page first.
            NameEntryDto? name = await _apiService.GetName(nameEntry);

            if (name == null)
            {
                return Redirect($"/entries?q={HttpUtility.UrlEncode(nameEntry)}");
            }

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
