using Core.Cache;

namespace Application.Cache
{
    public class RecentSearchesCache : IRecentSearchesCache
    {
        private int recencyLimit = 5;
        private int popularListLimit = 5;

        private readonly List<string> _searchedNamesCache;
        private readonly Dictionary<string, int> _searchFrequency;

        public RecentSearchesCache()
        {
            _searchedNamesCache = new List<string>();
            _searchFrequency = new Dictionary<string, int>();
        }

        public async Task<IEnumerable<string>> Get()
        {
            return await Task.FromResult(_searchedNamesCache.ToArray());
        }

        public async Task Stack(string name)
        {
            await Insert(name);
            int count = _searchedNamesCache.Count;
            if (count > recencyLimit)
            {
                _searchedNamesCache.RemoveAt(count - 1);
            }
        }

        private async Task Insert(string name)
        {
            if (_searchedNamesCache.Contains(name))
            {
                _searchedNamesCache.Remove(name);
            }
            _searchedNamesCache.Insert(0, name);
            await UpdateFrequency(name);
        }

        public async Task<bool> Remove(string name)
        {
            if (_searchedNamesCache.Contains(name))
            {
                _searchedNamesCache.Remove(name);
                _searchFrequency.Remove(name);
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<string>> GetMostPopular()
        {
            var frequency = GetNameWithSearchFrequency();
            return frequency.Select(item => item.Key).Take(popularListLimit);
        }

        private async Task UpdateFrequency(string name)
        {
            if (!_searchFrequency.ContainsKey(name))
            {
                _searchFrequency.Add(name, 0);
            }
            _searchFrequency[name]++;
        }

        private Dictionary<string, int> GetNameWithSearchFrequency()
        {
            return _searchFrequency
                        .OrderByDescending(item => item.Value)
                        .ToDictionary(item => item.Key, item => item.Value);
        }
    }
}
