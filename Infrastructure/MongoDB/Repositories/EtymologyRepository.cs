using Core.Entities;
using MongoDB.Driver;
using YorubaOrganization.Core.Entities.Partials;
using YorubaOrganization.Core.Events;
using YorubaOrganization.Core.Repositories;
using YorubaOrganization.Core.Tenants;
using YorubaOrganization.Infrastructure;
using YorubaOrganization.Infrastructure.Repositories;

namespace Infrastructure.MongoDB.Repositories;

public class EtymologyRepository(IMongoDatabaseFactory mongoDatabaseFactory, ITenantProvider tenantProvider) :
    MongoDBRepository<NameEntry>(mongoDatabaseFactory, tenantProvider, "NameEntries"), IEtymologyRepository
{
    public async Task<IDictionary<string, string>> GetLatestMeaningOf(IEnumerable<string> parts)
    {
        // We use secondary collation instead of primary because we want to ignore case but not diacritics.
        var options = SetCollationSecondary<AggregateOptions>(new AggregateOptions());
        var result = await RepoCollection.Aggregate(options)
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