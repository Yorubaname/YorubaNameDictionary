namespace Core.Cache
{
    public interface IRecentSearchesCache : ICache<string>
    {
        Task<IEnumerable<string>> GetMostPopular();
    }
}