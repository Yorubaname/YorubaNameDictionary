
namespace Application.Services
{
    public interface IEventPubService
    {
        Task PublishEvent(object theEvent);
    }
}
