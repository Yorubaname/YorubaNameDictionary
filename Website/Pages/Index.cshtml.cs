using Microsoft.Extensions.Localization;
using Website.Pages.Shared;
using Website.Resources;

namespace Website.Pages
{
    public class IndexModel : BasePageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            ILogger<IndexModel> logger,
            IStringLocalizer<Messages> localizer) : base(localizer)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
