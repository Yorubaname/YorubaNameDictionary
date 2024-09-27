namespace YorubaOrganization.Core.Repositories
{
    public interface IEtymologyRepository
    {
        Task<IDictionary<string, string>> GetLatestMeaningOf(IEnumerable<string> parts);
    }
}
