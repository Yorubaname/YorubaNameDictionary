using Core.Dto.Response;
using Core.Entities.NameEntry;
using Core.Enums;
using System.Linq.Expressions;

namespace Core.Repositories
{
    public interface INameEntryRepository
    {
        Task<NameEntry> FindById(string id);
        Task Create(NameEntry newName);
        Task Create(List<NameEntry> newName);
        Task<bool> DeleteAll();

        // TODO: This method should not be accessible. Too heavy on the DB
        Task<HashSet<NameEntry>> ListAll();

        Task<NameEntry?> FindByName(string name);

        Task<List<NameEntry>> FindByState(State state);

        Task<HashSet<NameEntry>> FindByNameStartingWithAndState(string alphabet, State state);

        Task<HashSet<NameEntry>> FindByNameStartingWithAnyAndState(IEnumerable<string> searchTerms, State state);

        Task<HashSet<NameEntry>> FindNameEntryByNameContainingAndState(string name, State state);

        Task<HashSet<NameEntry>> FindNameEntryByVariantsContainingAndState(string name, State state);

        Task<HashSet<NameEntry>> FindNameEntryByMeaningContainingAndState(string name, State state);

        Task<HashSet<NameEntry>> FindNameEntryByExtendedMeaningContainingAndState(string name, State state);

        Task<NameEntry?> FindByNameAndState(string name, State state);

        Task<int> CountByState(State state);

        Task Delete(string name);

        Task<bool> DeleteByNameAndState(string name, State state);

        Task<NameEntry?> Update(string originalName, NameEntry newEntry);

        Task<int> CountWhere(Expression<Func<NameEntry, bool>> filter);

        Task<List<NameEntry>> List(int pageNumber, int pageSize, Expression<Func<NameEntry, bool>>? filter = null);

        Task<NamesMetadataDto> GetMetadata();
    }
}
