namespace YorubaOrganization.Core.Cache
{
    public interface ISimpleCache
    {
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<T?> GetAsync<T>(string key);
    }
}
