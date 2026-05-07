using Application.Services.MultiLanguage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Words.Core.Dto.Request;
using Words.Core.Dto.Response;
using Words.Core.Entities;
using Words.Website.Pages.Shared;
using Words.Website.Resources;
using Words.Website.Services;
using YorubaOrganization.Core.Dto.Request;
using YorubaOrganization.Core.Dto.Response;

namespace Words.Website.Pages
{
    public class SubmitWordModel(
        IStringLocalizer<Messages> localizer,
        ILanguageService languageService,
        ApiService apiService,
        ILogger<SubmitWordModel> logger) : BasePageModel(localizer, languageService)
    {
        private readonly ApiService _apiService = apiService;

        [BindProperty(SupportsGet = true)]
        [FromQuery(Name = "missing")]
        public string? MissingWord { get; set; }

        [BindProperty(SupportsGet = true)]
        [FromQuery(Name = "meaning")]
        public string? SuggestedMeaning { get; set; }

        [BindProperty(SupportsGet = true)]
        [FromQuery(Name = "email")]
        public string? SuggestedEmail { get; set; }

        [BindProperty(SupportsGet = true)]
        [FromQuery(Name = "pos")]
        public int? SuggestedPartOfSpeech { get; set; }

        [BindProperty(SupportsGet = true)]
        [FromQuery(Name = "geo")]
        public string[] SuggestedGeoLocation { get; set; } = [];

        public GeoLocationDto[] GeoLocations { get; private set; } = [];

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGet()
        {
            GeoLocations = await _apiService.GetGeoLocations() ?? [];
        }

        public async Task<IActionResult> OnPost(
            string word,
            string suggestedMeaning,
            string? suggestedEmail,
            int partOfSpeech,
            string[] suggestedGeoLocation)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please complete all required fields and try again.";
                return RedirectToPage(new
                {
                    missing = word,
                    meaning = suggestedMeaning,
                    email = suggestedEmail,
                    pos = partOfSpeech,
                    geo = suggestedGeoLocation
                });
            }

            if (!Enum.IsDefined(typeof(PartOfSpeech), partOfSpeech))
            {
                ErrorMessage = "Please select a valid part of speech.";
                return RedirectToPage(new
                {
                    missing = word,
                    meaning = suggestedMeaning,
                    email = suggestedEmail,
                    pos = partOfSpeech,
                    geo = suggestedGeoLocation
                });
            }

            try
            {
                var dto = new CreateWordDto
                {
                    Word = word,
                    SubmittedBy = string.IsNullOrWhiteSpace(suggestedEmail) ? "Anonymous" : suggestedEmail,
                    PartOfSpeech = (PartOfSpeech)partOfSpeech,
                };

                if (!string.IsNullOrWhiteSpace(suggestedMeaning))
                {
                    dto.Definitions.Add(new DefinitionDto(suggestedMeaning, null, [], DateTime.UtcNow));
                }

                foreach (var geo in suggestedGeoLocation ?? [])
                {
                    var parts = geo.Split('.', 2);
                    var place = parts.Length > 1 ? parts[1] : geo;
                    var region = parts.Length > 0 ? parts[0] : geo;
                    dto.GeoLocation.Add(new CreateGeoLocationDto(place, region));
                }

                await _apiService.SuggestWordAsync(dto);
                SuccessMessage = "Thank you! Your word has been submitted for review.";
                return RedirectToPage();
            }
            catch(Exception ex)
            {
                ErrorMessage = "Something went wrong submitting your word. Please try again.";
                logger.LogError(ex, ErrorMessage);
                return RedirectToPage(new
                {
                    missing = word,
                    meaning = suggestedMeaning,
                    email = suggestedEmail,
                    pos = partOfSpeech,
                    geo = suggestedGeoLocation
                });
            }
        }
    }
}
