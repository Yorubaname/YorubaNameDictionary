using Hangfire.Dashboard;

namespace Infrastructure.Hangfire
{
    public class HangfireAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return context.GetHttpContext().Request.Host.Host == "localhost";
        }
    }
}
