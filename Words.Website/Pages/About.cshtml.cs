using Application.Services.MultiLanguage;
using Microsoft.Extensions.Localization;
using Words.Website.Pages.Shared;
using Words.Website.Resources;

namespace Words.Website.Pages
{
    public class AboutModel(IStringLocalizer<Messages> localizer, ILanguageService languageService) : BasePageModel(localizer, languageService)
    {
        public void OnGet()
        {
            // Set up any specific ViewData for the about page if needed
        }
    }
}
