using Core.Cache;

namespace Application.Cache
{
    public class RecentSearchesCache : InMemoryCache, IRecentSearchesCache
    {
        private int popularListLimit = 5;

        private readonly List<string> _theCache;

        public RecentSearchesCache() : base()
        {
        }


        public async Task<IEnumerable<string>> GetMostPopular()
        {
            var frequency = GetNameWithSearchFrequency();
            return frequency.Select(item => item.Key).Take(popularListLimit);
        }


        private Dictionary<string, int> GetNameWithSearchFrequency()
        {
            return _itemFrequency
                        .OrderByDescending(item => item.Value)
                        .ToDictionary(item => item.Key, item => item.Value);
        }
    }
}
