using Application.Services.MultiLanguage;
using Core.Dto.Response.Names;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Website.Pages.Shared;
using Website.Resources;
using Website.Services;
using YorubaOrganization.Application.Services;

namespace Website.Pages
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
        public NameEntryDto[] Names { get; set; } = [];


        public async Task<IActionResult> OnGet(string letter)
        {
            if (letter.Length > 2)
            {
                return RedirectToPage("Alphabet", new { letter = letter[..1] });
            }

            Letter = letter;
            Names = await _apiService.GetAllNamesByAlphabet(letter);

            // TODO Hafiz: The letters for Igbo names will probably be different than those for Yoruba names.
            Letters = YorubaAlphabetService.YorubaAlphabet;

            if ("g".Equals(letter, StringComparison.OrdinalIgnoreCase))
            {
                Names = Names.Where(name => !name.Name.StartsWith("gb", StringComparison.CurrentCultureIgnoreCase)).ToArray();
            }

            Count = Names.Length;

            return Page();
        }
    }
}
