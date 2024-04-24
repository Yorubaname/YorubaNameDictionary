namespace Core.Cache
{
    public interface IRecentSearchesCache
    {
        Task<IEnumerable<string>> Get();
        Task<IEnumerable<string>> GetMostPopular();
        Task Stack(string name);
        Task<bool> Remove(string name);
    }
}