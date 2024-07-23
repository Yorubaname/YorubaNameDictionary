using Core.Entities.NameEntry;
using Core.Entities.NameEntry.Collections;
using Core.Events;
using Core.Repositories;
using MongoDB.Driver;

namespace Infrastructure.MongoDB.Repositories;

public class EtymologyRepository : MongoDBRepository, IEtymologyRepository
{
    private readonly IMongoCollection<NameEntry> _nameEntryCollection;
    private readonly IEventPubService _eventPubService;

    public EtymologyRepository(
        IMongoDatabase database,
        IEventPubService eventPubService)
    {
        _nameEntryCollection = database.GetCollection<NameEntry>("NameEntries");
        _eventPubService = eventPubService;
    }

    public async Task<IDictionary<string, string>> GetLatestMeaningOf(IEnumerable<string> parts)
    {
        // We use secondary collation instead of primary because we want to ignore case but not diacritics.
        var options = SetCollationSecondary<AggregateOptions>(new AggregateOptions());
        var result = await _nameEntryCollection.Aggregate(options)
            .Unwind<NameEntry, UnwoundNameEntry>(x => x.Etymology)
            .Match(x => parts.Contains(x.Etymology.Part))
            .SortByDescending(x => x.Etymology.CreatedAt)
            .Group(x => x.Etymology.Part, g => new Etymology(g.Key, g.First().Etymology.Meaning) { })
            .ToListAsync();

        return result.ToDictionary(x => x.Part, x => x.Meaning);
    }

    private record UnwoundNameEntry(Etymology Etymology) 
    {
    }
}