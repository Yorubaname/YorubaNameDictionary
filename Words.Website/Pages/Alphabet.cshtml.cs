using Application.Services.MultiLanguage;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Words.Core.Dto.Response;
using Words.Website.Pages.Shared;
using Words.Website.Resources;
using Words.Website.Services;
using YorubaOrganization.Application.Services;

namespace Words.Website.Pages
{
    public class AlphabetModel(
        IStringLocalizer<Messages> localizer,
        ILanguageService languageService,
        ApiService apiService) : BasePageModel(localizer, languageService)
    {
        private readonly ApiService _apiService = apiService;

        public string Letter { get; set; } = string.Empty;
        public List<string> Letters { get; private set; } = [];
        public int Count { get; set; }
        public WordEntryDto[] Words { get; set; } = [];

        public async Task<IActionResult> OnGet(string letter)
        {
            if (string.IsNullOrWhiteSpace(letter))
            {
                return RedirectToPage("Index");
            }

            if (letter.Length > 2)
            {
                return RedirectToPage("Alphabet", new { letter = letter[..1] });
            }

            Letter = letter;
            Words = await _apiService.GetAllWordsByAlphabet(letter) ?? [];

            Letters = YorubaAlphabetService.YorubaAlphabet;

            if ("g".Equals(letter, StringComparison.OrdinalIgnoreCase))
            {
                Words = Words
                    .Where(word => !word.Word.StartsWith("gb", StringComparison.CurrentCultureIgnoreCase))
                    .ToArray();
            }

            Count = Words.Length;

            return Page();
        }
    }
}
