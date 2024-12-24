using Core.Entities;
using Core.Repositories;
using MongoDB.Driver;

namespace Infrastructure.MongoDB.Repositories;

public class SuggestedNameRepository : ISuggestedNameRepository
{
    private readonly IMongoCollection<SuggestedName> _suggestedNameCollection;

    public SuggestedNameRepository(IMongoDatabase database)
    {
        _suggestedNameCollection = database.GetCollection<SuggestedName>("SuggestedNames");
    }

    public async Task<Dictionary<string, int>> CountAsync()
    {
        var metaData = new Dictionary<string, int>();

        HashSet<string> uniqueNames = new();

        // Fetch all suggested names
        var suggestedNamesCursor = await _suggestedNameCollection
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
        await _suggestedNameCollection.InsertOneAsync(suggestedName);
        return suggestedName;
    }

    public async Task<SuggestedName> GetAsync(string id)
    {
        var filter = Builders<SuggestedName>.Filter.Eq("_id", id);
        var result = await _suggestedNameCollection.Find(filter).FirstOrDefaultAsync();
        return result;
    }

    public async Task<List<SuggestedName>> GetAllAsync()
    {
        return await _suggestedNameCollection.Find(FilterDefinition<SuggestedName>.Empty)
            .ToListAsync();
    }

    public async Task<bool> DeleteSuggestedNameAsync(string id)
    {
        var filter = Builders<SuggestedName>.Filter.Eq("_id", id);
        var result = await _suggestedNameCollection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    public async Task<bool> DeleteAllSuggestionsAsync()
    {
        var result = await _suggestedNameCollection.DeleteManyAsync(_ => true);

        return result.DeletedCount > 0;
    }
}
