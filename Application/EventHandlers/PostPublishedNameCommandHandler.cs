using Application.Events;
using MediatR;

namespace Application.EventHandlers
{
    public class PostPublishedNameCommandHandler : INotificationHandler<PostPublishedNameCommand>
    {
        private readonly IEventsQueue _eventsQueue;

        public PostPublishedNameCommandHandler(IEventsQueue eventsQueue)
        {
            _eventsQueue = eventsQueue;
        }

        public async Task Handle(PostPublishedNameCommand notification, CancellationToken cancellationToken)
        {
            await _eventsQueue.QueueEvent(notification);
        }
    }
}
