using Core.Entities;
using Core.Repositories;

namespace Application.Services;

public class SuggestedNamesService
{
    private readonly ISuggestedNameRepository _suggestedNameRepository;

    public SuggestedNamesService(ISuggestedNameRepository suggestedNameRepository)
    {
        _suggestedNameRepository = suggestedNameRepository;
    }

    public async Task<Dictionary<string, int>> CountAsync()
    {
        return await _suggestedNameRepository.CountAsync();
    }

    public async Task<SuggestedName> SuggestedNameAsync(SuggestedName suggestedName)
    {
        return await _suggestedNameRepository.SuggestedNameAsync(suggestedName);
    }

    public async Task<List<SuggestedName>> GetAllAsync()
    {
        return await _suggestedNameRepository.GetAllAsync();
    }

    public async Task<SuggestedName> GetAsync(string id)
    {
        return await _suggestedNameRepository.GetAsync(id);
    }

    public async Task<bool> DeleteSuggestedNameAsync(string id)
    {
        return await _suggestedNameRepository.DeleteSuggestedNameAsync(id);
    }

    public async Task<bool> DeleteAllSuggestionsAsync()
    {
        return await _suggestedNameRepository.DeleteAllSuggestionsAsync();
    }
}
