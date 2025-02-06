using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infrastructure.Redis
{
    public abstract class RedisCache(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisConfig> redisConfig)
    {
        protected readonly IDatabase Cache = connectionMultiplexer.GetDatabase(redisConfig.Value.DatabaseIndex);
    }
}
