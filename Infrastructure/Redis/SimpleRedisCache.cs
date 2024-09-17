using Ardalis.GuardClauses;
using Core.Cache;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
namespace Infrastructure.Redis
{
    public class SimpleRedisCache(IConnectionMultiplexer connectionMultiplexer, IOptions<RedisConfig> redisConfig) :
        RedisCache(connectionMultiplexer, redisConfig), ISimpleCache
    {
        public async Task<T?> GetAsync<T>(string key)
        {
            RedisValue theValue = await _cache.StringGetAsync(key);

            return theValue.IsNullOrEmpty ? default : ConvertToType<T>(theValue);
        }

        private static T ConvertToType<T>(RedisValue value)
        {
            if(typeof(T) == typeof(DateTimeOffset))
            {
                return (T)(object)DateTimeOffset.Parse(value.ToString());
            }
            return (T)Convert.ChangeType(value.ToString(), typeof(T));
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            await _cache.StringSetAsync(key, Guard.Against.Null(value)!.ToString(), expiry);
        }
    }
}
