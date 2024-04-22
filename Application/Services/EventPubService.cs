using Core.Events;
using MediatR;
using System.Reflection;

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
            Type? adapterType = Assembly.GetExecutingAssembly().GetType(typeof(T).FullName + "Adapter");

            if (adapterType == null)
            {
                throw new InvalidOperationException("Adapter type not found for " + typeof(T).FullName);
            }

            await _mediator.Publish(Activator.CreateInstance(adapterType, theEvent));
        }
    }
}
