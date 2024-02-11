namespace Core.Events
{
    public interface IEventPubService
    {
        Task PublishEvent(object theEvent);
    }
}
