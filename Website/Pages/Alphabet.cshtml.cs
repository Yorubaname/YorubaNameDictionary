using Application.Services;
using Core.Dto.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Website.Pages.Shared;
using Website.Resources;
using Website.Services;

namespace Website.Pages
{
    public class AlphabetModel(
        IStringLocalizer<Messages> localizer,
        ApiService apiService) : BasePageModel(localizer)
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
