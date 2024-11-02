using Ardalis.GuardClauses;
using MediatR;
using YorubaOrganization.Core.Events;

namespace Application.Services
{
    public class EventPubService : IEventPubService
    {
        private readonly IMediator _mediator;

        public EventPubService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task PublishEvent<T>(T theEvent)
        {
            Guard.Against.Null(theEvent, nameof(theEvent));
            await _mediator.Publish(theEvent);
        }
    }
}
