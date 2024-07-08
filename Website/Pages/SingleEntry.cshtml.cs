using Application.Services;
using Core.Dto.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Text.Json;
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
        public List<string> Alphabets { get; private set; } = [];
        public string Host { get; set; } = string.Empty;

        public string[] MostPopularNames { get; set; } = [];

        public async Task<IActionResult> OnGet(string nameEntry)
        {
            NameEntryDto? name = null;

            // TODO: Pass data between pages
            name ??= await _apiService.GetName(nameEntry);

            if (name == null)
            {
                return Redirect($"/entries?q={nameEntry}");
            }

            Name = name;
            Alphabets = YorubaAlphabetService.YorubaAlphabets;
            Host = $"{Request.Scheme}://{Request.Host}";

            var searchActivity = await _apiService.GetRecentStats();
            MostPopularNames = searchActivity.MostPopular;

            return Page();
        }
    }
}
