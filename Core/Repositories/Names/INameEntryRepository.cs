using Core.Entities;
using YorubaOrganization.Core.Enums;
using YorubaOrganization.Core.Repositories;

namespace Core.Repositories.Names
{
    public interface INameEntryRepository : IDictionaryEntryRepository<NameEntry>
    {
        Task<HashSet<NameEntry>> FindEntryByMeaningContainingAndState(string title, State state);
        Task<HashSet<NameEntry>> FindEntryByExtendedMeaningContainingAndState(string title, State state);
    }
}
