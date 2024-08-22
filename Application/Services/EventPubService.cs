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
            var adapterClassName = typeof(T).Name + "Adapter";
            Type? adapterType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name == adapterClassName);

            if (adapterType == null)
            {
                throw new InvalidOperationException("Adapter type not found for " + typeof(T).FullName);
            }

            var adapterEvent = Activator.CreateInstance(adapterType, theEvent)!;
            await _mediator.Publish(adapterEvent);
        }
    }
}
