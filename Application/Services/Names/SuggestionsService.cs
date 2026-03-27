using Core.Entities;
using Core.Repositories;

namespace Application.Services.Names;

public class SuggestionsService
{
    private readonly ISuggestedNameRepository _suggestedNameRepository;

    public SuggestionsService(ISuggestedNameRepository suggestedNameRepository)
    {
        _suggestedNameRepository = suggestedNameRepository;
    }

    public async Task<Dictionary<string, int>> CountAsync()
    {
        return await _suggestedNameRepository.CountAsync();
    }

    public async Task<SuggestedName> CreateAsync(SuggestedName suggestedName)
    {
        return await _suggestedNameRepository.CreateAsync(suggestedName);
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

    public async Task<BatchDeleteSuggestionsResult> DeleteSuggestedNamesBatchAsync(IEnumerable<string> names)
    {
        var requestedNames = names
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n.Trim())
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .ToArray();

        if (requestedNames.Length == 0)
        {
            return new BatchDeleteSuggestionsResult();
        }

        var deletedNames = await _suggestedNameRepository.DeleteSuggestedNamesBatchAsync(requestedNames);

        var notFoundNames = requestedNames
            .Except(deletedNames, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();

        return new BatchDeleteSuggestionsResult
        {
            DeletedItems = deletedNames,
            NotFoundItems = notFoundNames
        };
    }
}
