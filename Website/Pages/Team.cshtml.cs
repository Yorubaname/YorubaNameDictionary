using Microsoft.Extensions.Localization;
using Website.Pages.Shared;
using Website.Resources;
using Website.Services;

namespace Website.Pages
{
    public class TeamModel(IStringLocalizer<Messages> localizer, ApiService apiService) : StaticPageModel(localizer, apiService)
    {

    }
}
