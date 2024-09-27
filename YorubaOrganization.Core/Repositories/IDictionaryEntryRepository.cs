using System.Linq.Expressions;
using YorubaOrganization.Core.Dto.Response;
using YorubaOrganization.Core.Entities;
using YorubaOrganization.Core.Enums;

namespace YorubaOrganization.Core.Repositories
{
    public interface IDictionaryEntryRepository<T> where T : DictionaryEntry<T>
    {
        Task<T> FindById(string id);
        Task Create(T newName);
        Task Create(List<T> newName);
        Task<bool> DeleteAll();

        // TODO Later: This method should not be accessible. Too heavy on the DB
        Task<HashSet<T>> ListAll();

        Task<T?> FindByName(string name);
        Task<List<T>> FindByNames(string[] names);

        Task<List<T>> FindByState(State state);

        Task<HashSet<T>> FindByNameStartingWithAndState(string alphabet, State state);

        Task<HashSet<T>> FindByNameStartingWithAnyAndState(IEnumerable<string> searchTerms, State state);

        Task<HashSet<T>> FindNameEntryByNameContainingAndState(string name, State state);

        Task<HashSet<T>> FindNameEntryByVariantsContainingAndState(string name, State state);

        Task<HashSet<T>> FindNameEntryByMeaningContainingAndState(string name, State state);

        Task<HashSet<T>> FindNameEntryByExtendedMeaningContainingAndState(string name, State state);

        Task<T?> FindByNameAndState(string name, State state);

        Task<int> CountByState(State state);

        Task Delete(string name);

        Task DeleteMany(string[] name);

        Task<bool> DeleteByNameAndState(string name, State state);

        Task<T?> Update(string originalName, T newEntry);

        Task<int> CountWhere(Expression<Func<T, bool>> filter);

        Task<List<T>> List(int pageNumber, int pageSize, State? state, string? submittedBy);

        Task<MetadataResponse> GetMetadata();
        Task<IEnumerable<T>> GetAllNames(State? state, string? submittedBy);
    }
}
