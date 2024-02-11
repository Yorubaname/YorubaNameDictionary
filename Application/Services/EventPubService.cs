using Core.Events;

namespace Application.Services
{
    public class EventPubService : IEventPubService
    {
        public async Task PublishEvent(object theEvent)
        {
            // TODO Hafiz: Actually implement this.
            await Task.CompletedTask;
        }
    }
}
