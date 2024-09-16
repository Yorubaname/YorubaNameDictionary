using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Cache
{
    public interface ICacheService<T>
    {
        /// <summary>
        /// Retrieves a collection of items from the cache.
        /// </summary>
        /// <param name="key">The key under which the items are cached.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of items.</returns>
        Task<IEnumerable<T>> GetAsync(string key);

        /// <summary>
        /// Adds an item to the cache and updates its recency.
        /// </summary>
        /// <param name="key">The key under which the item is cached.</param>
        /// <param name="item">The item to be added to the cache.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StackAsync(string key, T item);

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="key">The key of the item to be removed.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the removal was successful.</returns>
        Task RemoveAsync(string key, string searchTerm);

        /// <summary>
        /// Retrieves the most popular items based on their frequency.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of the most popular items.</returns>
        Task<IEnumerable<string>> GetMostPopularAsync(string key);
    }
}
