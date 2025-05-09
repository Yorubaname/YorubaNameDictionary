﻿using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using YorubaOrganization.Core.Cache;
using YorubaOrganization.Core.Tenants;

namespace Infrastructure.Redis
{
    public class RedisRecentIndexesCache(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisConfig> redisConfig,
        ITenantProvider tenantProvider) : RedisCache(connectionMultiplexer, redisConfig), IRecentIndexesCache
    {
        private string Key => $"{tenantProvider.GetCurrentTenant()}_recent_indexes";
        private const int MaxItemsToReturn = 5;
        private const int MaxItemsToStore = 10;

        public async Task<IEnumerable<string>> Get()
        {
            var results = await Cache.SortedSetRangeByRankAsync(Key, 0, MaxItemsToReturn - 1, Order.Descending);
            return results.Select(r => r.ToString());
        }

        public async Task<bool> Remove(string item)
        {
            var tran = Cache.CreateTransaction();
            _ = tran.SortedSetRemoveAsync(Key, item);
            return await tran.ExecuteAsync();
        }

        public async Task Stack(string item)
        {
            // Use a Redis transaction to ensure atomicity of both operations
            var transaction = Cache.CreateTransaction();

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
