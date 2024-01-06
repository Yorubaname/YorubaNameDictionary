using Core.Entities.NameEntry;
using Core.Enums;

namespace Core.Repositories
{
    public interface INameEntryRepository
    {
        Task<NameEntry> FindByName(string name);

        Task<List<NameEntry>> FindByState(State state);

        Task<HashSet<NameEntry>> FindByNameStartingWithAndState(string alphabet, State state);

        Task<HashSet<NameEntry>> FindNameEntryByNameContainingAndState(string name, State state);

        Task<HashSet<NameEntry>> FindNameEntryByVariantsContainingAndState(string name, State state);

        Task<HashSet<NameEntry>> FindNameEntryByMeaningContainingAndState(string name, State state);

        Task<HashSet<NameEntry>> FindNameEntryByExtendedMeaningContainingAndState(string name, State state);

        Task<NameEntry> FindByNameAndState(string name, State state);

        Task<int> CountByState(State state);

        Task<bool> DeleteByNameAndState(string name, State state);
    }
}
