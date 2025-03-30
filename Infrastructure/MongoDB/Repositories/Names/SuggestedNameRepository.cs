using Core.Entities;
using Core.Repositories.Names;
using MongoDB.Driver;
using YorubaOrganization.Core.Tenants;
using YorubaOrganization.Infrastructure;
using YorubaOrganization.Infrastructure.Repositories;

namespace Infrastructure.MongoDB.Repositories.Names;

public class SuggestedNameRepository(IMongoDatabaseFactory databaseFactory, ITenantProvider tenantProvider) :
    MongoDBRepository<SuggestedName>(databaseFactory, tenantProvider, "SuggestedNames"), ISuggestedNameRepository
{
    public async Task<Dictionary<string, int>> CountAsync()
    {
        var metaData = new Dictionary<string, int>();

        HashSet<string> uniqueNames = [];

        // Fetch all suggested names
        var suggestedNamesCursor = await RepoCollection
            .FindSync(FilterDefinition<SuggestedName>.Empty).ToListAsync();

        // Add unique names to HashSet
        foreach (var name in suggestedNamesCursor)
        {
            uniqueNames.Add(name.Name!);
        }

        metaData.Add("totalSuggestedNames", uniqueNames.Count);

        return metaData;
    }

    public async Task<SuggestedName> CreateAsync(SuggestedName suggestedName)
    {
        await RepoCollection.InsertOneAsync(suggestedName);
        return suggestedName;
    }

    public async Task<SuggestedName> GetAsync(string id)
    {
        var filter = Builders<SuggestedName>.Filter.Eq("_id", id);
        var result = await RepoCollection.Find(filter).FirstOrDefaultAsync();
        return result;
    }

    public async Task<List<SuggestedName>> GetAllAsync()
    {
        return await RepoCollection.Find(FilterDefinition<SuggestedName>.Empty)
            .ToListAsync();
    }

    public async Task<bool> DeleteSuggestedNameAsync(string id)
    {
        var filter = Builders<SuggestedName>.Filter.Eq("_id", id);
        var result = await RepoCollection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    public async Task<bool> DeleteAllSuggestionsAsync()
    {
        var result = await RepoCollection.DeleteManyAsync(_ => true);

        return result.DeletedCount > 0;
    }
}
