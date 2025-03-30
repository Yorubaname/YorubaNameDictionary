using Application.Events;
using Application.Services.Names;
using MediatR;
using YorubaOrganization.Core;

namespace Application.EventHandlers
{
    public class PostPublishedNameCommandHandler(ITwitterService twitterService) : INotificationHandler<PostPublishedNameCommand>
    {
        public async Task Handle(PostPublishedNameCommand notification, CancellationToken cancellationToken)
        {
            if (notification.Tenant == Languages.YorubaLanguage)
            {
                await twitterService.PostNewNameAsync(notification.Name, notification.Meaning, cancellationToken);
            }
        }
    }
}
