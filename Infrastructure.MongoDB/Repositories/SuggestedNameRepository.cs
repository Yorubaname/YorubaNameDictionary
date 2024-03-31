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

    public async Task<Dictionary<string, int>> Count()
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
}
