using Application.Events;
using Core.Cache;
using MediatR;

namespace Application.EventHandlers
{
    public class NameIndexedEventHandler : INotificationHandler<NameIndexedAdapter>
    {
        public IRecentIndexesCache _recentIndexesCache;

        public NameIndexedEventHandler(IRecentIndexesCache recentIndexesCache) 
        {
            _recentIndexesCache = recentIndexesCache;
        }

        public async Task Handle(NameIndexedAdapter notification, CancellationToken cancellationToken)
        {
            await _recentIndexesCache.Stack(notification.Name);
        }
    }
}
