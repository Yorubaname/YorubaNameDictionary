using Words.Core.Entities;
using YorubaOrganization.Core.Repositories;

namespace Core.Repositories.Words
{
    public interface IWordEntryRepository : IDictionaryEntryRepository<WordEntry>
    {
        //Task<HashSet<NameEntry>> FindEntryByMeaningContainingAndState(string title, State state);
        //Task<HashSet<NameEntry>> FindEntryByExtendedMeaningContainingAndState(string title, State state);
    }
}
