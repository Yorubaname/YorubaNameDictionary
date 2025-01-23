using Microsoft.Extensions.Localization;
using Website.Pages.Shared;
using Website.Resources;
using Website.Services;
using Website.Services.MultiLanguage;

namespace Website.Pages
{
    public class AboutModel(IStringLocalizer<Messages> localizer, ILanguageService languageService, ApiService apiService) : 
        StaticPageModel(localizer, languageService, apiService)
    {
    }
}
