using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure.Redis
{
    public static class DependencyInjection
    {
        public static IServiceCollection SetupRedis(this IServiceCollection services, string redisConnectionString)
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
            return services;
        }
    }
}
