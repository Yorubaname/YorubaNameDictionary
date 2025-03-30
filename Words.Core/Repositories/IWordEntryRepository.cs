using Words.Core.Entities;
using YorubaOrganization.Core.Enums;
using YorubaOrganization.Core.Repositories;

namespace Words.Core.Repositories
{
    public interface IWordEntryRepository : IDictionaryEntryRepository<WordEntry>
    {
        Task<HashSet<WordEntry>> FindEntryByDefinitionsContentContainingAndState(string title, State state);
    }
}
