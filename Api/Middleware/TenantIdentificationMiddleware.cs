using YorubaOrganization.Core;

namespace Api.Middleware
{
    public class TenantIdentificationMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantIdentificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var tenant = context.Request.Headers["X-Language"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(tenant))
            {
                tenant = Languages.YorubaLanguage;
            }

            context.Items["Tenant"] = tenant;
            await _next(context);
        }
    }
}
