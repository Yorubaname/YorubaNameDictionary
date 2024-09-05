using Ardalis.GuardClauses;
using System.Collections.Concurrent;

namespace Application.Events
{
    public class InMemoryEventsQueue : IEventsQueue
    {
        private readonly ConcurrentDictionary<Type, ConcurrentQueue<object>> _eventQueues = new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> must not be null.</exception>
        public async Task QueueEvent<T>(T item)
        {
            Guard.Against.Null(item, nameof(item));

            var eventType = typeof(T);
            var queue = _eventQueues.GetOrAdd(eventType, _ => new ConcurrentQueue<object>());
            queue.Enqueue(item);

            // Simulate async operation
            await Task.CompletedTask;
        }

        public async Task<T?> Pop<T>()
        {
            var eventType = typeof(T);
            if (_eventQueues.TryGetValue(eventType, out var queue) && queue.TryDequeue(out var item))
            {
                return (T)item;
            }

            // No events of type T available
            return await Task.FromResult(default(T));
        }
    }
}
