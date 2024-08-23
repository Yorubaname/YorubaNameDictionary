using Application.Events;
using MediatR;
using System.Collections.Concurrent;

namespace Application.EventHandlers
{
    public class PostPublishedNameCommandHandler : INotificationHandler<PostPublishedNameCommand>
    {
        private readonly ConcurrentQueue<PostPublishedNameCommand> _nameQueue;

        public PostPublishedNameCommandHandler(ConcurrentQueue<PostPublishedNameCommand> nameQueue)
        {
            _nameQueue = nameQueue;
        }

        public Task Handle(PostPublishedNameCommand notification, CancellationToken cancellationToken)
        {
            // Enqueue the indexed name for processing by the BackgroundService
            _nameQueue.Enqueue(notification);

            // Return a completed task, so it doesn't block the main thread
            return Task.CompletedTask;
        }
    }
}
