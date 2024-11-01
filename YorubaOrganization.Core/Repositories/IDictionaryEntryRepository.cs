using System.Linq.Expressions;
using YorubaOrganization.Core.Dto.Response;
using YorubaOrganization.Core.Entities;
using YorubaOrganization.Core.Enums;

namespace YorubaOrganization.Core.Repositories
{
    public interface IDictionaryEntryRepository<T> where T : DictionaryEntry<T>
    {
        Task<T> FindById(string id);
        Task Create(T newEntry);
        Task Create(List<T> newEntries);
        Task<bool> DeleteAll();

        // TODO Later: This method should not be accessible. Too heavy on the DB
        Task<HashSet<T>> ListAll();

        Task<T?> FindByTitle(string title);
        Task<List<T>> FindByTitles(string[] titles);

        Task<List<T>> FindByState(State state);

        Task<HashSet<T>> FindByTitleStartingWithAndState(string alphabet, State state);

        Task<HashSet<T>> FindByTitleStartingWithAnyAndState(IEnumerable<string> searchTerms, State state);

        Task<HashSet<T>> FindEntryByTitleContainingAndState(string title, State state);

        Task<HashSet<T>> FindEntryByVariantsContainingAndState(string title, State state);

        Task<HashSet<T>> FindEntryByMeaningContainingAndState(string title, State state);

        Task<HashSet<T>> FindEntryByExtendedMeaningContainingAndState(string title, State state);

        Task<T?> FindByTitleAndState(string title, State state);

        Task<int> CountByState(State state);

        Task Delete(string title);

        Task DeleteMany(string[] titles);

        Task<bool> DeleteByTitleAndState(string title, State state);

        Task<T?> Update(string originalTitle, T newEntry);

        Task<int> CountWhere(Expression<Func<T, bool>> filter);

        Task<List<T>> List(int pageNumber, int pageSize, State? state, string? submittedBy);

        Task<MetadataResponse> GetMetadata();
        Task<IEnumerable<T>> GetAllEntries(State? state, string? submittedBy);
    }
}
