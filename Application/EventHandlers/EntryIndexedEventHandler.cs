using Application.Events;
using Core.Events;
using MediatR;
using YorubaOrganization.Core.Cache;

namespace Application.EventHandlers
{
    public class NameIndexedEventHandler : INotificationHandler<NameIndexed>
    {
        public IRecentIndexesCache _recentIndexesCache;
        private readonly IMediator _mediator;

        public NameIndexedEventHandler(
            IRecentIndexesCache recentIndexesCache,
            IMediator mediator) 
        {
            _recentIndexesCache = recentIndexesCache;
            _mediator = mediator;
        }

        public async Task Handle(NameIndexed notification, CancellationToken cancellationToken)
        {
            await _recentIndexesCache.Stack(notification.Name);
            await _mediator.Publish(new PostPublishedNameCommand(notification.Name, notification.Meaning), cancellationToken);
        }
    }
}
