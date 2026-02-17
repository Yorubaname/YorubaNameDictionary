using Application.Services.MultiLanguage;
using Microsoft.Extensions.Localization;
using Words.Website.Resources;
using Words.Website.Services;

namespace Words.Website.Pages.Shared
{
    public abstract class StaticPageModel(IStringLocalizer<Messages> localizer, ILanguageService languageService, ApiService apiService) :
        BasePageModel(localizer, languageService)
    {
        protected readonly ApiService _apiService = apiService;

        public long WordCount { get; private set; }

        public virtual async Task OnGet()
        {
            var indexedWordCount = await _apiService.GetIndexedWordCount();
            WordCount = indexedWordCount?.TotalPublishedWords ?? 0;
        }
    }
}