using Core.Entities;

namespace Core.Repositories;

public interface ISuggestedNameRepository
{
    Task<Dictionary<string, int>> CountAsync();
    Task<SuggestedName> SuggestedNameAsync(SuggestedName suggestedName);
    Task<List<SuggestedName>> GetAllAsync();
    Task<SuggestedName> DeleteSuggestedNameAsync(string id);
    Task<bool> DeleteAllSuggestionsAsync();
}
