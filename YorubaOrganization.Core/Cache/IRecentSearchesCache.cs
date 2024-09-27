namespace YorubaOrganization.Core.Cache
{
    public interface IRecentSearchesCache : ISetBasedCache<string>
    {
        Task<IEnumerable<string>> GetMostPopular();
    }
}