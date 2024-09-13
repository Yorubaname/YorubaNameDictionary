using Hangfire.Dashboard;

namespace Api.Utilities
{
    public class HangfireAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return context.GetHttpContext().Request.Host.Host == "localhost";
        }
    }
}
