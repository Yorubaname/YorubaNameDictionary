using Core.Entities;
using Core.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using YorubaOrganization.Core.Enums;
using YorubaOrganization.Core.Events;
using YorubaOrganization.Core.Utilities;
using YorubaOrganization.Infrastructure.Repositories;

namespace Infrastructure.MongoDB.Repositories;

public sealed class NameEntryRepository : DictionaryEntryRepository<NameEntry>, INameEntryRepository
{
    private const string CollectionName = "NameEntries";
    public NameEntryRepository(
        IMongoDatabase database,
        IEventPubService eventPubService) : base(CollectionName, database, eventPubService)
    {
    }

    public async Task<HashSet<NameEntry>> FindEntryByExtendedMeaningContainingAndState(string name, State state)
    {
        var filter = Builders<NameEntry>.Filter.Regex(ne => ne.ExtendedMeaning,
            new BsonRegularExpression(name.ReplaceYorubaVowelsWithPattern(), "i")) &
            Builders<NameEntry>.Filter.Eq(ne => ne.State, state);
        var result = await EntryCollection.Find(filter).ToListAsync();
        return new HashSet<NameEntry>(result);
    }

    protected override UpdateDefinition<NameEntry> GenerateCustomUpdateStatement(NameEntry newEntry) => Builders<NameEntry>.Update
                    .Set(ne => ne.Meaning, newEntry.Meaning)
                    .Set(ne => ne.ExtendedMeaning, newEntry.ExtendedMeaning)
                    .Set(ne => ne.FamousPeople, newEntry.FamousPeople);
}