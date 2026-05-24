using MongoDB.Driver;
using YorubaOrganization.Core.Events;
using YorubaOrganization.Core.Tenants;
using YorubaOrganization.Infrastructure.Repositories;
using YorubaOrganization.Infrastructure;
using Words.Core.Entities;
using YorubaOrganization.Core.Enums;
using YorubaOrganization.Core.Utilities;
using MongoDB.Bson;
using Words.Core.Repositories;
using Words.Core.Dto.Response;

namespace Infrastructure.MongoDB.Repositories.Words
{
    public class WordRepository : DictionaryEntryRepository<WordEntry>, IWordEntryRepository
    {
        private const string CollectionName = "Words";
        private const string YorubaDefinitionPlaceholder = "{{YORUBA-DEFINITION-PLACEHOLDER}}";
        private const string DefinitionsNeedsReviewStateIndexName = "idx_words_definitions_needsreview_state";
        private static readonly object IndexCreationLock = new();
        private static bool _DoesWordDefinitionsNeedingReviewIndexExist;

        public WordRepository(IMongoDatabaseFactory databaseFactory, ITenantProvider tenantProvider, IEventPubService eventPubService)
            : base(CollectionName, databaseFactory, tenantProvider, eventPubService)
        {
            EnsureIndexesCreated();
        }

        private sealed class UnwoundWordDefinition
        {
            public string Title { get; set; } = string.Empty;
            public State State { get; set; }
            public Definition Definitions { get; set; } = null!;
        }

        public async Task<int> CountByStateAsync(State state)
        {
            var filter = Builders<WordEntry>.Filter.Eq(ne => ne.State, state);
            return (int)await RepoCollection.CountDocumentsAsync(filter);
        }

        public async Task<List<WordEntry>> FindByStateAsync(State state)
        {
            var filter = Builders<WordEntry>.Filter.Eq(ne => ne.State, state);
            return await RepoCollection.Find(filter).ToListAsync();
        }

        public async Task<WordEntry?> GetByIdAsync(string id)
        {
            var filter = Builders<WordEntry>.Filter.Eq(ne => ne.Id, id);
            return await RepoCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<WordEntry?> AcceptSuggestionAsync(string id)
        {
            var suggestedFilter = Builders<WordEntry>.Filter.Eq(w => w.Id, id)
                & Builders<WordEntry>.Filter.Eq(w => w.State, State.SUGGESTED);

            var suggestedWord = await RepoCollection.Find(suggestedFilter).FirstOrDefaultAsync();
            if (suggestedWord == null)
            {
                return null;
            }

            suggestedWord.State = State.NEW;

            var replaceResult = await RepoCollection.ReplaceOneAsync(suggestedFilter, suggestedWord);
            if (replaceResult.MatchedCount == 0)
            {
                return null;
            }

            return suggestedWord;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<WordEntry>.Filter.Eq(ne => ne.Id, id);
            var result = await RepoCollection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<string[]> DeleteSuggestedWordsBatchAsync(IEnumerable<string> words)
        {
            var requestedWords = words
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .Select(w => w.Trim())
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .ToArray();

            if (requestedWords.Length == 0)
            {
                return [];
            }

            var matchingTitlesFilter = Builders<WordEntry>.Filter.In(w => w.Title, requestedWords)
                & Builders<WordEntry>.Filter.Eq(w => w.State, State.SUGGESTED);

            var existingWords = (await RepoCollection
                    .Find(matchingTitlesFilter)
                    .Project(w => w.Title)
                    .ToListAsync())
                .OfType<string>()
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .ToArray();

            if (existingWords.Length == 0)
            {
                return [];
            }

            var deleteFilter = Builders<WordEntry>.Filter.In(w => w.Title, existingWords)
                & Builders<WordEntry>.Filter.Eq(w => w.State, State.SUGGESTED);

            await RepoCollection.DeleteManyAsync(deleteFilter);

            return existingWords;
        }

        public async Task<int> DeleteByStateAsync(State state)
        {
            var filter = Builders<WordEntry>.Filter.Eq(ne => ne.State, state);
            var result = await RepoCollection.DeleteManyAsync(filter);
            return (int)result.DeletedCount;
        }

        // TODO YDict: Test that this definition content search works as expected.
        public async Task<HashSet<WordEntry>> FindEntryByDefinitionsContentContainingAndStateAsync(string title, State state)
        {
            var filter = Builders<WordEntry>.Filter.ElemMatch(ne => ne.Definitions,
            Builders<Definition>.Filter.Regex(d => d.Content, new BsonRegularExpression(title.ReplaceYorubaVowelsWithPattern(), "i")))
            & Builders<WordEntry>.Filter.Eq(ne => ne.State, state);
            var result = await RepoCollection.Find(filter).ToListAsync();
            return [.. result];
        }

        public async Task<IDictionary<string, string[]>> GetEnglishDefinitionsOfAsync(IEnumerable<string> words)
        {
            var requestedWords = words
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .Select(w => w.Trim())
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .ToArray();

            if (requestedWords.Length == 0)
            {
                return new Dictionary<string, string[]>();
            }

            var matchingWords = await RepoCollection
                .Find(Builders<WordEntry>.Filter.In(w => w.Title, requestedWords), SetCollationSecondary<FindOptions>(new FindOptions()))
                .Project(w => new
                {
                    w.Title,
                    w.Definitions
                })
                .ToListAsync();

            var result = requestedWords.ToDictionary(
                keySelector: word => word,
                elementSelector: _ => Array.Empty<string>());

            foreach (var matchingWord in matchingWords)
            {
                var englishDefinitions = matchingWord.Definitions
                    .Where(d => !string.IsNullOrWhiteSpace(d.EnglishTranslation))
                    .Select(d => d.EnglishTranslation!.Trim())
                    .Distinct(StringComparer.CurrentCultureIgnoreCase)
                    .ToArray();

                result[matchingWord.Title] = englishDefinitions;
            }

            return result;
        }

        public async Task<IDictionary<string, string>> AddEnglishDefinitionsAsync(IDictionary<string, string> definitionsByWord, string? currentUser = null)
        {
            var requests = definitionsByWord
                .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key) && !string.IsNullOrWhiteSpace(kvp.Value))
                .GroupBy(kvp => kvp.Key.Trim(), StringComparer.CurrentCultureIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.Last().Value.Trim(),
                    StringComparer.CurrentCultureIgnoreCase);

            var statuses = new Dictionary<string, string>();

            if (requests.Count == 0)
            {
                return statuses;
            }

            var requestedWords = requests.Keys.ToArray();
            var existingWords = await RepoCollection
                .Find(Builders<WordEntry>.Filter.In(w => w.Title, requestedWords), SetCollationSecondary<FindOptions>(new FindOptions()))
                .ToListAsync();

            var existingByTitle = existingWords.ToDictionary(w => w.Title, StringComparer.CurrentCultureIgnoreCase);

            foreach (var request in requests)
            {
                if (!existingByTitle.TryGetValue(request.Key, out var wordEntry))
                {
                    await CreateWord(request, currentUser);
                    statuses[request.Key] = "created";
                    continue;
                }

                var definitionExists = wordEntry.Definitions.Any(d =>
                    !string.IsNullOrWhiteSpace(d.EnglishTranslation)
                    && string.Equals(d.EnglishTranslation.Trim(), request.Value, StringComparison.CurrentCultureIgnoreCase));

                if (definitionExists)
                {
                    statuses[request.Key] = "skipped-duplicate";
                    continue;
                }

                wordEntry.Definitions.Add(new Definition
                {
                    Content = YorubaDefinitionPlaceholder,
                    EnglishTranslation = request.Value,
                    NeedsReview = true
                });

                if (!string.IsNullOrWhiteSpace(currentUser))
                {
                    wordEntry.UpdatedBy = currentUser;
                }

                await RepoCollection.ReplaceOneAsync(
                    Builders<WordEntry>.Filter.Eq(w => w.Id, wordEntry.Id),
                    wordEntry);

                statuses[request.Key] = "created";
            }

            return statuses;
        }

        public async Task<List<WordDefinitionNeedsReviewDto>> GetWordsWithDefinitionsNeedingReviewAsync(int page, int count)
        {
            page = Math.Max(1, page);
            count = Math.Max(1, count);
            var skip = (page - 1) * count;

            var aggregated = await RepoCollection
                .Aggregate(SetCollationSecondary<AggregateOptions>(new AggregateOptions()))
                .Unwind<WordEntry, UnwoundWordDefinition>(w => w.Definitions)
                .Match(x => x.Definitions.NeedsReview == true)
                .Group(
                    x => new { x.Title, x.State },
                    g => new WordDefinitionNeedsReviewDto(
                        g.Key.State,
                        g.Key.Title,
                        g.Max(x => x.Definitions.CreatedAt),
                        g.Count()))
                .SortByDescending(x => x.LastDefinitionAddedAt)
                .Skip(skip)
                .Limit(count)
                .ToListAsync();

            return aggregated;
        }

        private async Task CreateWord(KeyValuePair<string, string> request, string? currentUser)
        {
            var newWordEntry = new WordEntry
            {
                Title = request.Key,
                State = State.NEW,
                Definitions = [new Definition
                {
                    Content = YorubaDefinitionPlaceholder,
                    EnglishTranslation = request.Value,
                    NeedsReview = true
                }]
            };

            if (!string.IsNullOrWhiteSpace(currentUser))
            {
                newWordEntry.CreatedBy = currentUser;
                newWordEntry.UpdatedBy = currentUser;
            }

            await Create(newWordEntry);
        }

        private bool EnsureIndexesCreated()
        {
            if (_DoesWordDefinitionsNeedingReviewIndexExist)
            {
                return true;
            }

            lock (IndexCreationLock)
            {
                if (_DoesWordDefinitionsNeedingReviewIndexExist)
                {
                    return true;
                }

                var indexKeys = Builders<WordEntry>.IndexKeys
                    .Ascending("Definitions.NeedsReview")
                    .Ascending(x => x.State);

                var indexModel = new CreateIndexModel<WordEntry>(
                    indexKeys,
                    new CreateIndexOptions { Name = DefinitionsNeedsReviewStateIndexName });

                RepoCollection.Indexes.CreateOne(indexModel);
                _DoesWordDefinitionsNeedingReviewIndexExist = true;
                return true;
            }
        }

        // TODO YDict: Implement the custom FindBy methods (which are commented out in the interface) here (based on definitions).

        protected override UpdateDefinition<WordEntry> GenerateCustomUpdateStatement(WordEntry newEntry) => Builders<WordEntry>.Update
                        .Set(ne => ne.PartOfSpeech, newEntry.PartOfSpeech)
                        .Set(ne => ne.Style, newEntry.Style)
                        .Set(ne => ne.GrammaticalFeature, newEntry.GrammaticalFeature)
                        .Set(ne => ne.Definitions, newEntry.Definitions);
    }
}
