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

namespace Infrastructure.MongoDB.Repositories.Words
{
    public class WordRepository(IMongoDatabaseFactory databaseFactory, ITenantProvider tenantProvider, IEventPubService eventPubService) : DictionaryEntryRepository<WordEntry>(CollectionName, databaseFactory, tenantProvider, eventPubService), IWordEntryRepository
    {
        private const string CollectionName = "Words";

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

        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<WordEntry>.Filter.Eq(ne => ne.Id, id);
            var result = await RepoCollection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<int> DeleteByStateAsync(State state)
        {
            var filter = Builders<WordEntry>.Filter.Eq(ne => ne.State, state);
            var result = await RepoCollection.DeleteManyAsync(filter);
            return (int)result.DeletedCount;
        }

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
