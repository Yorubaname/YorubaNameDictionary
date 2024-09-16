using Core.Cache;
using StackExchange.Redis;

namespace Infrastructure.Redis
{
    public class RedisRecentSearchesCache : IRecentSearchesCache
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _cache;
        private const string RecentSearchesKey = "recent_searches";
        private const string PopularSearchesKey = "popular_searches";
        private const int MaxRecentSearchToReturn = 5;
        private const int MaxRecentSearches = 10;
        private const int MaxPopularSearches = 10;

        public RedisRecentSearchesCache(IConnectionMultiplexer connectionMultiplexer)
        {
            _redis = connectionMultiplexer;
            _cache = _redis.GetDatabase();
        }

        public async Task<IEnumerable<string>> Get()
        {
            var results = await _cache.SortedSetRangeByRankAsync(RecentSearchesKey, 0, -1, Order.Descending);
            return results.Take(MaxRecentSearchToReturn).Select(r => r.ToString());
        }

        public async Task<IEnumerable<string>> GetMostPopular()
        {
            var results = await _cache.SortedSetRangeByRankAsync(PopularSearchesKey, 0, -1, Order.Descending);
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
            string searchTerm = item.ToString();

            // Use a Redis transaction to ensure atomicity of both operations
            var transaction = _cache.CreateTransaction();

            // Add the search term to the front of the Redis list
            _ = transaction.SortedSetAddAsync(RecentSearchesKey, item, DateTime.UtcNow.Ticks);
            _ = transaction.SortedSetRemoveRangeByRankAsync(RecentSearchesKey, 0, -(MaxRecentSearches + 1));

            // Increment the search term's frequency in the specific sorted set
            _ = transaction.SortedSetIncrementAsync(PopularSearchesKey, searchTerm, 1);
            _ = transaction.SortedSetRemoveRangeByRankAsync(PopularSearchesKey, 0, -(MaxPopularSearches + 1));

            // Execute the transaction
            bool committed = await transaction.ExecuteAsync();
            if (!committed)
            {
                throw new Exception("Transaction failed");
            }
        }
    }
}
