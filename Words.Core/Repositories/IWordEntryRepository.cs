using Words.Core.Entities;
using YorubaOrganization.Core.Enums;
using YorubaOrganization.Core.Repositories;

namespace Words.Core.Repositories
{
    public interface IWordEntryRepository : IDictionaryEntryRepository<WordEntry>
    {
        Task<HashSet<WordEntry>> FindEntryByDefinitionsContentContainingAndState(string title, State state);
        Task<int> CountByStateAsync(State state);
        Task<List<WordEntry>> FindByStateAsync(State state);
        Task<WordEntry?> GetByIdAsync(string id);
        Task<bool> DeleteAsync(string id);
        Task<int> DeleteByStateAsync(State state);
    }
}
