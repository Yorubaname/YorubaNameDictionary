using Microsoft.AspNetCore.Localization;

namespace Website.Middleware
{
    public class LocalizationCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public LocalizationCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var feature = context.Features.Get<IRequestCultureFeature>();
            var requestCulture = feature?.RequestCulture;

            if (requestCulture != null)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30),
                    HttpOnly = false,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                };

                context.Response.Cookies.Append("lang", requestCulture.Culture.Name, cookieOptions); // We're adding this because it is used by the UI.
                context.Response.Cookies.Append("culture", CookieRequestCultureProvider.MakeCookieValue(requestCulture), cookieOptions);
            }

            await _next(context);
        }
    }

}
