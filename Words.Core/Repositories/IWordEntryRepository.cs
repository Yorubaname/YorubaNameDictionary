using Words.Core.Entities;
using Words.Core.Dto.Response;
using YorubaOrganization.Core.Enums;
using YorubaOrganization.Core.Repositories;

namespace Words.Core.Repositories
{
    public interface IWordEntryRepository : IDictionaryEntryRepository<WordEntry>
    {
        Task<HashSet<WordEntry>> FindEntryByDefinitionsContentContainingAndStateAsync(string title, State state);
        Task<List<WordEntry>> GetPublishedWithEtymologyPageAsync(int page, int count);
        Task<IDictionary<string, string[]>> GetEnglishDefinitionsOfAsync(IEnumerable<string> words);
        Task<IDictionary<string, string>> AddEnglishDefinitionsAsync(IDictionary<string, string> definitionsByWord, string? currentUser = null);
        Task<IDictionary<string, string>> AddEnglishDefinitionsAsync(IDictionary<string, string[]> definitionsByWord, string? currentUser = null);
        Task<WordDefinitionsNeedingReviewPageDto> GetWordsWithDefinitionsNeedingReviewAsync(int page, int count);
        Task<int> CountByStateAsync(State state);
        Task<List<WordEntry>> FindByStateAsync(State state);
        Task<WordEntry?> GetByIdAsync(string id);
        Task<WordEntry?> AcceptSuggestionAsync(string id);
        Task<bool> DeleteAsync(string id);
        Task<string[]> DeleteSuggestedWordsBatchAsync(IEnumerable<string> words);
        Task<int> DeleteByStateAsync(State state);
    }
}
