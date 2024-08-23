using Application.Events;
using Core.Cache;
using MediatR;

namespace Application.EventHandlers
{
    public class NameIndexedEventHandler : INotificationHandler<NameIndexedAdapter>
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

        public async Task Handle(NameIndexedAdapter notification, CancellationToken cancellationToken)
        {
            await _recentIndexesCache.Stack(notification.Name);
            await _mediator.Publish(new PostPublishedNameCommand(notification.Name), cancellationToken);
        }
    }
}
