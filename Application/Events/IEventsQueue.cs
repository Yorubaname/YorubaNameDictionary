namespace Application.Events
{
    public interface IEventsQueue
    {
        Task QueueEvent<T>(T item);
        Task<T?> Pop<T>();
    }
}
