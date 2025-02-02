using Application.Events;
using Core.Events;
using MediatR;
using YorubaOrganization.Core.Cache;

namespace Application.EventHandlers
{
    public class NameIndexedEventHandler(IRecentIndexesCache recentIndexesCache, IMediator mediator) :
        INotificationHandler<NameIndexed>
    {
        public IRecentIndexesCache _recentIndexesCache = recentIndexesCache;

        public async Task Handle(NameIndexed notification, CancellationToken cancellationToken)
        {
            await _recentIndexesCache.Stack(notification.Name);
            await mediator.Publish(new PostPublishedNameCommand(notification.Name, notification.Meaning), cancellationToken);
        }
    }
}
