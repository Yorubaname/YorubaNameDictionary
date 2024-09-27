namespace YorubaOrganization.Core.Events
{
    public interface IEventPubService
    {
        Task PublishEvent<T>(T theEvent);
    }
}
