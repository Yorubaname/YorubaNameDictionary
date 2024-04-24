using Core.Cache;

namespace Application.Cache
{
    public class RecentIndexesCache : IRecentIndexesCache
    {
        public async Task<IEnumerable<string>> Get()
        {
            throw new NotImplementedException();
            // TODO Hafiz: Implement pulling from cache.
            // Return all cached names.
        }
    }
}
