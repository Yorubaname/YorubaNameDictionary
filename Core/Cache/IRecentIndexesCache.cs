namespace Core.Cache
{
    public interface IRecentIndexesCache
    {
        Task<IEnumerable<string>> Get();
    }
}