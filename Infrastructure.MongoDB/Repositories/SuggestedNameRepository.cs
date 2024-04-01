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

        metaData.Add("uniqueSuggestedNames", uniqueNames.Count);

        return metaData;
    }

    public async Task<SuggestedName> SuggestedNameAsync(SuggestedName suggestedName)
    {
        await _suggestedNameCollection.InsertOneAsync(suggestedName);
        // todo: retun the name only if saved
        return suggestedName;
    }

    public async Task<List<SuggestedName>> GetAllAsync()
    {
        return await _suggestedNameCollection.Find(FilterDefinition<SuggestedName>.Empty)
            .ToListAsync();
    }

    public async Task<SuggestedName> DeleteSuggestedNameAsync(string id)
    {
        var suggestedName = await _suggestedNameCollection.FindOneAndDeleteAsync(s => s.Id == id);

        return suggestedName;
    }
}
