namespace YorubaOrganization.Core.Cache
{
    public interface ISetBasedCache<T>
    {
        Task<IEnumerable<T>> Get();
        Task Stack(T item);
        Task<bool> Remove(T item);
    }
}
