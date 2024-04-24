using Core.Cache;

namespace Application.Cache
{
    public abstract class InMemoryCache : ICache<string>
    {
        protected readonly List<string> _itemCache;
        protected readonly Dictionary<string, int> _itemFrequency;

        private int recencyLimit = 5;

        public InMemoryCache()
        {
            _itemCache = new List<string>();
            _itemFrequency = new Dictionary<string, int>();
        }

        public async Task<IEnumerable<string>> Get()
        {
            return await Task.FromResult(_itemCache.ToArray());
        }

        public async Task Stack(string name)
        {
            await Insert(name);
            int count = _itemCache.Count;
            if (count > recencyLimit)
            {
                _itemCache.RemoveAt(count - 1);
            }
        }

        private async Task Insert(string name)
        {
            if (_itemCache.Contains(name))
            {
                _itemCache.Remove(name);
            }
            _itemCache.Insert(0, name);
            await UpdateFrequency(name);
        }

        public async Task<bool> Remove(string name)
        {
            if (_itemCache.Contains(name))
            {
                _itemCache.Remove(name);
                _itemFrequency.Remove(name);
                return true;
            }
            return false;
        }

        private async Task UpdateFrequency(string name)
        {
            if (!_itemFrequency.ContainsKey(name))
            {
                _itemFrequency.Add(name, 0);
            }
            _itemFrequency[name]++;
        }
    }
}
