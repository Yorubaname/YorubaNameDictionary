using Microsoft.Extensions.Localization;
using Website.Pages.Shared;
using Website.Resources;
using Website.Services;

namespace Website.Pages
{
    public class AboutModel(IStringLocalizer<Messages> localizer, ApiService apiService) : BasePageModel(localizer)
    {
        private readonly ApiService _apiService = apiService;

        public int NameCount { get; private set; }

        public async Task OnGet()
        {
            var indexedNameCount = await _apiService.GetIndexedNameCount();
            NameCount = indexedNameCount.TotalPublishedNames;
        }
    }
}
