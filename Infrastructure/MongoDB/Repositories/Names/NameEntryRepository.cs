using Core.Entities;
using Core.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using YorubaOrganization.Core.Enums;
using YorubaOrganization.Core.Events;
using YorubaOrganization.Core.Tenants;
using YorubaOrganization.Core.Utilities;
using YorubaOrganization.Infrastructure;
using YorubaOrganization.Infrastructure.Repositories;

namespace Infrastructure.MongoDB.Repositories.Names;

public sealed class NameEntryRepository(IMongoDatabaseFactory databaseFactory, ITenantProvider tenantProvider, IEventPubService eventPubService) : DictionaryEntryRepository<NameEntry>(CollectionName, databaseFactory, tenantProvider, eventPubService), INameEntryRepository
{
    private const string CollectionName = "NameEntries";

    public async Task<List<NameEntry>> GetPublishedWithEtymologyPageAsync(int page, int count)
    {
        page = Math.Max(1, page);
        count = Math.Max(1, count);
        var skip = (page - 1) * count;

        var filter = Builders<NameEntry>.Filter.Eq(ne => ne.State, State.PUBLISHED);

        return await RepoCollection
            .Find(filter)
            .SortBy(ne => ne.Id)
            .Skip(skip)
            .Limit(count)
            .Project(ne => new NameEntry
            {
                Id = ne.Id,
                Title = ne.Title,
                Etymology = ne.Etymology
            })
            .ToListAsync();
    }

    public async Task<HashSet<NameEntry>> FindEntryByMeaningContainingAndState(string title, State state)
    {
        var filter = Builders<NameEntry>.Filter.Regex(ne => ne.Meaning,
            new BsonRegularExpression(title.ReplaceYorubaVowelsWithPattern(), "i")) & 
            Builders<NameEntry>.Filter.Eq(ne => ne.State, state);
        var result = await RepoCollection.Find(filter).ToListAsync();
        return [.. result];
    }

    public async Task<HashSet<NameEntry>> FindEntryByExtendedMeaningContainingAndState(string name, State state)
    {
        var filter = Builders<NameEntry>.Filter.Regex(ne => ne.ExtendedMeaning,
            new BsonRegularExpression(name.ReplaceYorubaVowelsWithPattern(), "i")) &
            Builders<NameEntry>.Filter.Eq(ne => ne.State, state);
        var result = await RepoCollection.Find(filter).ToListAsync();
        return [.. result];
    }

    protected override UpdateDefinition<NameEntry> GenerateCustomUpdateStatement(NameEntry newEntry) => Builders<NameEntry>.Update
                    .Set(ne => ne.Meaning, newEntry.Meaning)
                    .Set(ne => ne.ExtendedMeaning, newEntry.ExtendedMeaning)
                    .Set(ne => ne.FamousPeople, newEntry.FamousPeople);
}