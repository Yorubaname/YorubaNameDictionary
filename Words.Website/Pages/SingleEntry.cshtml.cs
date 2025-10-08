using Application.Services.MultiLanguage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Web;
using Words.Core.Dto.Response;
using Words.Website.Pages.Shared;
using Words.Website.Resources;
using Words.Website.Services;

namespace Words.Website.Pages
{
    public class SingleEntryModel(
        IStringLocalizer<Messages> localizer,
        ILanguageService languageService,
        ApiService apiService) : BasePageModel(localizer, languageService)
    {
        private readonly ApiService _apiService = apiService;

        public WordEntryDto Word { get; set; } = new();
        public List<string> Letters { get; private set; } = [];
        public string[] MostPopular { get; set; } = [];

        public async Task<IActionResult> OnGet(string wordEntry)
        {
            // Try to get word from API
            WordEntryDto? word = await _apiService.GetWord(wordEntry);

            if (word == null)
            {
                return Redirect($"/search?q={HttpUtility.UrlEncode(wordEntry)}");
            }

            ViewData["SocialTitle"] = word.Word;
            ViewData["SocialPath"] = $"/entries/{HttpUtility.UrlEncode(word.Word)}";
            ViewData["SocialDescription"] = word.Definitions?.FirstOrDefault()?.Content ?? "";

            Word = word;

            // Initialize alphabet letters (simplified version)
            Letters = GetYorubaAlphabet();

            var searchActivity = await _apiService.GetRecentStats();
            MostPopular = searchActivity?.MostPopular ?? [];

            return Page();
        }

        private static List<string> GetYorubaAlphabet()
        {
            return new List<string>
            {
                "A", "B", "D", "E", "Ẹ", "F", "G", "GB", "H", "I", "J", "K", "KP", "L", "M",
                "N", "O", "Ọ", "P", "R", "S", "Ṣ", "T", "U", "W", "Y"
            };
        }
    }
}