using Core.Cache;
using StackExchange.Redis;

namespace Infrastructure.Redis
{
    public class RedisRecentIndexesCache : IRecentIndexesCache
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _cache;
        private const string Key = "recent_indexes";
        private const int MaxItemsToReturn = 5;
        private const int MaxItemsToStore = 10;

        public RedisRecentIndexesCache(IConnectionMultiplexer connectionMultiplexer)
        {
            _redis = connectionMultiplexer;
            _cache = _redis.GetDatabase();
        }

        public async Task<IEnumerable<string>> Get()
        {
            var results = await _cache.SortedSetRangeByRankAsync(Key, 0, MaxItemsToReturn -1, Order.Descending);
            return results.Select(r => r.ToString());
        }

        public async Task<bool> Remove(string item)
        {
            var tran = _cache.CreateTransaction();
            _ = tran.SortedSetRemoveAsync(Key, item);
            return await tran.ExecuteAsync();
        }

        public async Task Stack(string item)
        {
            // Use a Redis transaction to ensure atomicity of both operations
            var transaction = _cache.CreateTransaction();

            // Add the search term to the front of the Redis list
            _ = transaction.SortedSetAddAsync(Key, item, DateTime.UtcNow.Ticks);
            _ = transaction.SortedSetRemoveRangeByRankAsync(Key, 0, -(MaxItemsToStore + 1));

            // Execute the transaction
            bool committed = await transaction.ExecuteAsync();
            if (!committed)
            {
                throw new Exception("Redis Transaction failed");
            }
        }
    }
}
