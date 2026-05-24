using Core.Events;
using Microsoft.Extensions.Logging;
using Words.Core.Dto.Response;
using Words.Core.Entities;
using Words.Core.Repositories;
using YorubaOrganization.Application.Services;
using YorubaOrganization.Core.Events;
using YorubaOrganization.Core.Tenants;

namespace Application.Services.Words
{
    public class WordEntryService(
        IWordEntryRepository wordEntryRepository,
        IEventPubService eventPubService,
        ITenantProvider tenantProvider,
        ILogger<WordEntryService> logger) :
        DictionaryEntryService<WordEntry>(wordEntryRepository, eventPubService, tenantProvider, logger)
    {
        private readonly IEventPubService _eventPubService = eventPubService;
        private readonly IWordEntryRepository _wordEntryRepository = wordEntryRepository;
        private readonly string _currentTenant = tenantProvider.GetCurrentTenant();

        public Task<IDictionary<string, string[]>> GetEnglishDefinitionsOf(IEnumerable<string> words)
        {
            return _wordEntryRepository.GetEnglishDefinitionsOfAsync(words);
        }

        public Task<IDictionary<string, string>> AddEnglishDefinitionsAsync(IDictionary<string, string> definitionsByWord, string? currentUser = null)
        {
            return _wordEntryRepository.AddEnglishDefinitionsAsync(definitionsByWord, currentUser);
        }

        public Task<List<WordDefinitionNeedsReviewDto>> GetWordsWithDefinitionsNeedingReviewAsync(int page, int count)
        {
            return _wordEntryRepository.GetWordsWithDefinitionsNeedingReviewAsync(page, count);
        }

        public async Task<List<WordEntry>> BulkUpdateWords(List<WordEntry> wordEntries)
        {
            var updatedWords = new List<WordEntry>();

            // TODO Later: Update all words in one batch
            foreach (var wordEntry in wordEntries)
            {
                var updated = await UpdateEntryWithUnpublish(wordEntry);

                if (updated != null)
                {
                    updatedWords.Add(updated);
                }
                else
                {
                    await _eventPubService.PublishEvent(new NonExistingEntryUpdateAttempted(wordEntry.Title, _currentTenant));
                }
            }
            return updatedWords;
        }

        public async override Task PublishEntry(WordEntry wordEntry, string username)
        {
            // TODO YDict: Check that the nullability of two fields below does not break the system
            await base.PublishEntry(wordEntry, username, w => w.PartOfSpeech, w => w.Definitions, w => w.Style, w => w.GrammaticalFeature);

            // TODO Later: Use the outbox pattern to enforce event publishing after the DB update.
            var theDefinition = wordEntry.Definitions.FirstOrDefault();

            if (theDefinition != null)
            {
                await _eventPubService.PublishEvent(new WordIndexed(wordEntry.Title, theDefinition.Content, theDefinition.EnglishTranslation, _currentTenant));
            }
        }
    }
}