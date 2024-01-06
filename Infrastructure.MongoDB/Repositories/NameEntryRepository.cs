using Core.Entities.NameEntry;
using Core.Enums;
using Core.Repositories;
using MongoDB.Driver;

public class NameEntryRepository : INameEntryRepository
{
    private readonly IMongoCollection<NameEntry> _nameEntryCollection;

    public NameEntryRepository(IMongoDatabase database)
    {
        _nameEntryCollection = database.GetCollection<NameEntry>("NameEntries");
    }

    public async Task<int> CountByState(State state)
    {
        var count = await _nameEntryCollection.CountDocumentsAsync(ne => ne.State == state);
        return (int)count;
    }

    public async Task<bool> DeleteByNameAndState(string name, State state)
    {
        var deleteResult = await _nameEntryCollection.DeleteOneAsync(ne => ne.Name == name && ne.State == state);
        return deleteResult.DeletedCount > 0;
    }

    public async Task<NameEntry> FindByName(string name)
    {
        return await _nameEntryCollection.Find(ne => ne.Name == name).SingleOrDefaultAsync();
    }

    public async Task<NameEntry> FindByNameAndState(string name, State state)
    {
        return await _nameEntryCollection.Find(ne => ne.Name == name && ne.State == state).SingleOrDefaultAsync();
    }

    public async Task<HashSet<NameEntry>> FindByNameStartingWithAndState(string alphabet, State state)
    {
        var filter = Builders<NameEntry>.Filter.Regex(ne => ne.Name, new MongoDB.Bson.BsonRegularExpression($"^{alphabet}"));
        var result = await _nameEntryCollection.Find(filter & Builders<NameEntry>.Filter.Eq(ne => ne.State, state)).ToListAsync();
        return new HashSet<NameEntry>(result);
    }

    public async Task<List<NameEntry>> FindByState(State state)
    {
        return await _nameEntryCollection.Find(ne => ne.State == state).ToListAsync();
    }

    public async Task<HashSet<NameEntry>> FindNameEntryByExtendedMeaningContainingAndState(string name, State state)
    {
        var filter = Builders<NameEntry>.Filter.Where(ne =>
        ne.ExtendedMeaning != null && ne.ExtendedMeaning.Contains(name) && ne.State == state);
        var result = await _nameEntryCollection.Find(filter).ToListAsync();
        return new HashSet<NameEntry>(result);
    }

    public async Task<HashSet<NameEntry>> FindNameEntryByMeaningContainingAndState(string name, State state)
    {
        var filter = Builders<NameEntry>.Filter.Where(ne => ne.Meaning.Contains(name) && ne.State == state);
        var result = await _nameEntryCollection.Find(filter).ToListAsync();
        return new HashSet<NameEntry>(result);
    }

    public async Task<HashSet<NameEntry>> FindNameEntryByNameContainingAndState(string name, State state)
    {
        var filter = Builders<NameEntry>.Filter.Where(ne => ne.Name.Contains(name) && ne.State == state);
        var result = await _nameEntryCollection.Find(filter).ToListAsync();
        return new HashSet<NameEntry>(result);
    }

    public async Task<HashSet<NameEntry>> FindNameEntryByVariantsContainingAndState(string name, State state)
    {
        var filter = Builders<NameEntry>.Filter.Where(ne => ne.Variants.Contains(name) && ne.State == state);
        var result = await _nameEntryCollection.Find(filter).ToListAsync();
        return new HashSet<NameEntry>(result);
    }
}
