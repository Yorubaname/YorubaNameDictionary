using Core.Entities;

namespace Core.Repositories;

public interface ISuggestedNameRepository
{
    Task<Dictionary<string, int>> CountAsync();
    Task<SuggestedName> CreateAsync(SuggestedName suggestedName);
    Task<List<SuggestedName>> GetAllAsync();
    Task<SuggestedName> GetAsync(string id);
    Task<bool> DeleteSuggestedNameAsync(string id);
    Task<bool> DeleteAllSuggestionsAsync();
}
