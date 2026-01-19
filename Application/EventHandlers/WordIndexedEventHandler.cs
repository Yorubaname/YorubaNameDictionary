using Core.Events;
using MediatR;
using YorubaOrganization.Core.Cache;

namespace Application.EventHandlers
{
    public class WordIndexedEventHandler(IRecentIndexesCache recentIndexesCache) :
        INotificationHandler<WordIndexed>
    {
        public IRecentIndexesCache _recentIndexesCache = recentIndexesCache;

        public async Task Handle(WordIndexed notification, CancellationToken cancellationToken)
        {
            await _recentIndexesCache.Stack(notification.Word);
        }
    }
}