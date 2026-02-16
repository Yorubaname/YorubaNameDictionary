using Application.Services.MultiLanguage;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Words.Website.Resources;

namespace Words.Website.Pages.Shared
{
    public class BasePageModel(IStringLocalizer<Messages> localizer, ILanguageService languageService) : PageModel
    {
        protected readonly IStringLocalizer<Messages> _localizer = localizer;
        private readonly ILanguageService _languageService = languageService;

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);

            var host = $"{Request.Scheme}://{Request.Host}";

            ViewData["Description"] = "YorubaWord - Multimedia Dictionary of Yoruba Words";
            ViewData["BaseURL"] = host;
            ViewData["SocialPath"] = string.Empty;
            ViewData["SocialTitle"] = "YorubaWord";
            ViewData["SocialDescription"] = _localizer["word-count-tagline", 10000, _languageService.LanguageDisplay];
        }
    }
}