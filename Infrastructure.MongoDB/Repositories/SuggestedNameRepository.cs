using Core.Dto.Request;
using Core.Dto.Response;
using Core.Entities;
using Core.Repositories;
using MongoDB.Bson;
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
        // Todo use automapper
        //SuggestedName.Id = ObjectId.GenerateNewId().ToString()

        await _suggestedNameCollection.InsertOneAsync(suggestedName);

        return suggestedName;
    }
}
