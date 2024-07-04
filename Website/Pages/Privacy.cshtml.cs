using Microsoft.Extensions.Localization;
using Website.Pages.Shared;
using Website.Resources;

namespace Website.Pages
{
    public class PrivacyModel : BasePageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(
            ILogger<PrivacyModel> logger,
            IStringLocalizer<Messages> localizer
            ) : base(localizer)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }

}
