using Application.Services.MultiLanguage;
using Microsoft.Extensions.Localization;
using Website.Pages.Shared;
using Website.Resources;
using Website.Services;

namespace Website.Pages
{
    public class TermsModel(IStringLocalizer<Messages> localizer, ILanguageService languageService, ApiService apiService) :
        StaticPageModel(localizer, languageService, apiService)
    {
    }
}
