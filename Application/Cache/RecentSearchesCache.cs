using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Cache
{
    public class RecentSearchesCache
    {
        public async Task<IEnumerable<string>> Get()
        {
            throw new NotImplementedException();
            // TODO Hafiz: Implement pulling from cache.
            // Return all cached names.
        }

        public async Task<IEnumerable<string>> GetMostPopular()
        {
            throw new NotImplementedException();
            // TODO Hafiz: Implement pulling from cache.
        }
    }
}
