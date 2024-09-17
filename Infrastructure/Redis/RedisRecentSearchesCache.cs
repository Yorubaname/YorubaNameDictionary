using Core.Cache;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infrastructure.Redis
{
    public class RedisRecentSearchesCache(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisConfig> redisConfig) : RedisCache(connectionMultiplexer, redisConfig), IRecentSearchesCache
    {
        private const string RecentSearchesKey = "recent_searches";
        private const string PopularSearchesKey = "popular_searches";
        private static readonly DateTime StartDate;
        private const int MaxItemsToReturn = 5;
        private const int MaxRecentSearches = 10;

        static RedisRecentSearchesCache()
        {
            StartDate = new(2024, 9, 17); // Do not change
        }

        public async Task<IEnumerable<string>> Get()
        {
            var results = await _cache.SortedSetRangeByRankAsync(RecentSearchesKey, 0, MaxItemsToReturn -1, Order.Descending);
            return results.Select(r => r.ToString());
        }

        public async Task<IEnumerable<string>> GetMostPopular()
        {
            var results = await _cache.SortedSetRangeByRankAsync(PopularSearchesKey, 0, MaxItemsToReturn -1, Order.Descending);
            return results.Select(r => r.ToString());
        }

        public async Task<bool> Remove(string item)
        {
            var tran = _cache.CreateTransaction();
            _ = tran.SortedSetRemoveAsync(RecentSearchesKey, item);
            _ = tran.SortedSetRemoveAsync(PopularSearchesKey, item);
            return await tran.ExecuteAsync();
        }

        public async Task Stack(string item)
        {
            // Use a Redis transaction to ensure atomicity of both operations
            var transaction = _cache.CreateTransaction();

            // Add the search term to the front of the Redis list
            _ = transaction.SortedSetAddAsync(RecentSearchesKey, item, DateTime.UtcNow.Ticks);
            _ = transaction.SortedSetRemoveRangeByRankAsync(RecentSearchesKey, 0, -(MaxRecentSearches + 1));

            // TODO: Do a periodic caching, like daily where the most popular items from the previous period are brought forward into the next day
            var currentScore = (await _cache.SortedSetScoreAsync(PopularSearchesKey, item)) ?? 0;
            _ = transaction.SortedSetAddAsync(PopularSearchesKey, item, (int)++currentScore + GetNormalizedTimestamp());

            // Execute the transaction
            bool committed = await transaction.ExecuteAsync();
            if (!committed)
            {
                throw new Exception("Redis Transaction failed");
            }
        }

        static double GetNormalizedTimestamp()
        {
            TimeSpan timeSinceStartDate = DateTime.Now - StartDate;
            return timeSinceStartDate.TotalSeconds / 10_000_000_000; // It will take over 100 years for this value to grow to 1.
        }
    }
}
