using Application.Events;
using Application.Services;
using MediatR;

namespace Application.EventHandlers
{
    public class PostPublishedNameCommandHandler : INotificationHandler<PostPublishedNameCommand>
    {
        private readonly ITwitterService _twitterService;

        public PostPublishedNameCommandHandler(
            ITwitterService twitterService)
        {
            _twitterService = twitterService;

        }

        public async Task Handle(PostPublishedNameCommand notification, CancellationToken cancellationToken)
        {
            await _twitterService.PostNewNameAsync(notification.Name, notification.Meaning, cancellationToken);
        }
    }
}
