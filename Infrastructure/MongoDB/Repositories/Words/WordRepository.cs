using MongoDB.Driver;
using YorubaOrganization.Core.Events;
using YorubaOrganization.Core.Tenants;
using YorubaOrganization.Infrastructure.Repositories;
using YorubaOrganization.Infrastructure;
using Words.Core.Entities;
using Core.Repositories.Words;
using YorubaOrganization.Core.Enums;
using YorubaOrganization.Core.Utilities;
using MongoDB.Bson;

namespace Infrastructure.MongoDB.Repositories.Words
{
    public class WordRepository(IMongoDatabaseFactory databaseFactory, ITenantProvider tenantProvider, IEventPubService eventPubService) : DictionaryEntryRepository<WordEntry>(CollectionName, databaseFactory, tenantProvider, eventPubService), IWordEntryRepository
    {
        private const string CollectionName = "Words";

        // TODO YDict: Test that this definition content search works as expected.
        public async Task<HashSet<WordEntry>> FindEntryByDefinitionsContentContainingAndState(string title, State state)
        {
            var filter = Builders<WordEntry>.Filter.ElemMatch(ne => ne.Definitions,
            Builders<Definition>.Filter.Regex(d => d.Content, new BsonRegularExpression(title.ReplaceYorubaVowelsWithPattern(), "i")))
            & Builders<WordEntry>.Filter.Eq(ne => ne.State, state);
            var result = await RepoCollection.Find(filter).ToListAsync();
            return [.. result];
        }

        // TODO YDict: Implement the custom FindBy methods (which are commented out in the interface) here (based on definitions).

        protected override UpdateDefinition<WordEntry> GenerateCustomUpdateStatement(WordEntry newEntry) => Builders<WordEntry>.Update
                        .Set(ne => ne.PartOfSpeech, newEntry.PartOfSpeech)
                        .Set(ne => ne.Style, newEntry.Style)
                        .Set(ne => ne.GrammaticalFeature, newEntry.GrammaticalFeature)
                        .Set(ne => ne.Definitions, newEntry.Definitions);
    }
}
