using Words.Core.Entities;
using Words.Core.Repositories;
using YorubaOrganization.Core.Enums;

namespace Application.Services.Words;

public class SuggestionsService
{
    private readonly IWordEntryRepository _wordRepository;

    public SuggestionsService(IWordEntryRepository wordRepository)
    {
        _wordRepository = wordRepository;
    }

    public async Task<Dictionary<string, int>> CountAsync()
    {
        var count = await _wordRepository.CountByStateAsync(State.SUGGESTED);
        return new Dictionary<string, int> { { "totalSuggestedWords", count } };
    }

    public async Task CreateAsync(WordEntry wordEntry)
    {
        wordEntry.State = State.SUGGESTED;
        await _wordRepository.Create(wordEntry);
    }

    public async Task<List<WordEntry>> GetAllAsync()
    {
        return await _wordRepository.FindByStateAsync(State.SUGGESTED);
    }

    public async Task<WordEntry?> GetAsync(string id)
    {
        var entry = await _wordRepository.GetByIdAsync(id);
        return entry != null && entry.State == State.SUGGESTED ? entry : null;
    }

    public async Task<bool> DeleteSuggestedNameAsync(string id)
    {
        var entry = await _wordRepository.GetByIdAsync(id);
        if (entry != null && entry.State == State.SUGGESTED)
        {
            return await _wordRepository.DeleteAsync(id);
        }
        return false;
    }

    public async Task<bool> DeleteAllSuggestionsAsync()
    {
        var deletedCount = await _wordRepository.DeleteByStateAsync(State.SUGGESTED);
        return deletedCount > 0;
    }

    public async Task<BatchDeleteSuggestionsResult> DeleteSuggestedWordsBatchAsync(IEnumerable<string> words)
    {
        var requestedWords = words
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .Select(w => w.Trim())
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .ToArray();

        if (requestedWords.Length == 0)
        {
            return new BatchDeleteSuggestionsResult();
        }

        var deletedWords = await _wordRepository.DeleteSuggestedWordsBatchAsync(requestedWords);

        var notFoundWords = requestedWords
            .Except(deletedWords, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();

        return new BatchDeleteSuggestionsResult
        {
            DeletedItems = deletedWords,
            NotFoundItems = notFoundWords
        };
    }
}
