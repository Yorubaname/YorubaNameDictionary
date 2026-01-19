namespace Words.Website.Middleware
{
    public class LocalizationCookieMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var culture = context.Request.Query["lang"].FirstOrDefault();
            if (!string.IsNullOrEmpty(culture))
            {
                context.Response.Cookies.Append("culture", $"c={culture}|uic={culture}");
            }

            await next(context);
        }
    }
}