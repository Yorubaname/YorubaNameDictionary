using Application.Events;
using Application.Services;
using MediatR;

namespace Application.EventHandlers
{
    public class PostPublishedNameCommandHandler(
        ITwitterService twitterService) : INotificationHandler<PostPublishedNameCommand>
    {
        public async Task Handle(PostPublishedNameCommand notification, CancellationToken cancellationToken)
        {
            await twitterService.PostNewNameAsync(notification.Name, notification.Meaning, cancellationToken);
        }
    }
}
