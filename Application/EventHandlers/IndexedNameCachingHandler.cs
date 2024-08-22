using Application.Events;
using Core.Cache;
using MediatR;

namespace Application.EventHandlers
{
    public class IndexedNameCachingHandler : INotificationHandler<NameIndexedAdapter>
    {
        public IRecentIndexesCache _recentIndexesCache;

        public IndexedNameCachingHandler(IRecentIndexesCache recentIndexesCache) 
        {
            _recentIndexesCache = recentIndexesCache;
        }

        public async Task Handle(NameIndexedAdapter notification, CancellationToken cancellationToken)
        {
            await _recentIndexesCache.Stack(notification.Name);
        }
    }
}
