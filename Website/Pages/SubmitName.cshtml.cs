using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Website.Pages.Shared;
using Website.Resources;
using Website.Services;
using YorubaOrganization.Core.Dto.Response;

namespace Website.Pages
{
    public class SubmitNameModel(IStringLocalizer<Messages> localizer, ApiService apiService) : BasePageModel(localizer)
    {
        [BindProperty(SupportsGet = true)]
        [FromQuery(Name = "missing")]
        public string MissingName { get; set; } = string.Empty;
        public GeoLocationDto[] GeoLocations { get; private set; } = [];
        public ApiService _apiService = apiService;

        public async Task OnGet()
        {
            GeoLocations = await _apiService.GetGeoLocations();
        }
    }
}
