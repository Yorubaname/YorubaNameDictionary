using Application.Services.MultiLanguage;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Website.Resources;

namespace Website.Pages.Shared
{
    public class BasePageModel(IStringLocalizer<Messages> localizer, ILanguageService languageService) : PageModel
    {
        protected readonly IStringLocalizer<Messages> _localizer = localizer;
        private readonly ILanguageService _languageService = languageService;

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);

            var host = $"{Request.Scheme}://{Request.Host}";

            // Some of the strings below should be internationalized.
            ViewData["Description"] = _languageService.SocialName;
            ViewData["BaseURL"] = host;
            ViewData["SocialPath"] = string.Empty; // HomePage path
            ViewData["SocialTitle"] = _languageService.SocialName;
            ViewData["SocialDescription"] = _localizer["name-count-tagline", 10000, _languageService.LanguageDisplay];
        }
    }
}
