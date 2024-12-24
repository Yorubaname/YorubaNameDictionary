using Ardalis.GuardClauses;
using Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using YorubaOrganization.Core.Cache;

namespace Infrastructure.Redis
{
    public static class DependencyInjection
    {
        private const string SectionName = "Redis";

        public static IServiceCollection SetupRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedisConfig>(configuration.GetRequiredSection(SectionName));
            var redisConnectionString = Guard.Against.NullOrEmpty(configuration.GetConnectionString(SectionName));
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
            services.AddSingleton<ISimpleCache, SimpleRedisCache>();
            return services;
        }
    }
}
