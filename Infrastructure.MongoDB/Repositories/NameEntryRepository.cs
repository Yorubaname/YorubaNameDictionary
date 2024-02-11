using Core.Dto;
using Core.Entities.NameEntry;
using Core.Enums;
using Core.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.MongoDB.Repositories;

public class NameEntryRepository : INameEntryRepository
{
    private readonly IMongoCollection<NameEntry> _nameEntryCollection;

    public NameEntryRepository(IMongoDatabase database)
    {
        _nameEntryCollection = database.GetCollection<NameEntry>("NameEntries");
    }

    public async Task<NameEntry> FindById(string id)
    {
        return await _nameEntryCollection.Find(x => x.Id == id).SingleOrDefaultAsync();
    }

    public async Task<bool> DeleteAll()
    {
        var deleteResult = await _nameEntryCollection.DeleteManyAsync(FilterDefinition<NameEntry>.Empty);
        return deleteResult.DeletedCount > 0;
    }
    
    
    public async Task Create(NameEntry entry)
    {
        entry.Id = ObjectId.GenerateNewId().ToString();
        await _nameEntryCollection.InsertOneAsync(entry);
    }

    public async Task Create(List<NameEntry> entries)
    {
        entries.ForEach(entry =>  entry.Id = ObjectId.GenerateNewId().ToString()!);
        await _nameEntryCollection.InsertManyAsync(entries);
    }

    public async Task<int> CountByState(State state)
    {
        return (int)await CountWhere(ne => ne.State == state);
    }

    public async Task<bool> DeleteByNameAndState(string name, State state)
    {
        var deleteResult = await _nameEntryCollection.DeleteOneAsync(ne => ne.Name == name && ne.State == state);
        return deleteResult.DeletedCount > 0;
    }

    public async Task<HashSet<NameEntry>> ListAll()
    {
        var allEntries = await _nameEntryCollection.Find(_ => true).ToListAsync();
        return new HashSet<NameEntry>(allEntries);
    }

    public async Task<NameEntry?> FindByName(string name)
    {
        var filter = Builders<NameEntry>.Filter.Eq("Name", name);
        var options = new FindOptions
        {
            Collation = new Collation("en", strength: CollationStrength.Primary)
        };

        return await _nameEntryCollection.Find(filter, options).SingleOrDefaultAsync();
    }

    public async Task<NameEntry?> FindByNameAndState(string name, State state)
    {
        return await _nameEntryCollection.Find(ne => ne.Name == name && ne.State == state).SingleOrDefaultAsync();
    }

    public async Task<HashSet<NameEntry>> FindByNameStartingWithAndState(string alphabet, State state)
    {
        var filter = Builders<NameEntry>.Filter.Regex(ne => ne.Name, new BsonRegularExpression($"^{alphabet}"));
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

    public async Task<NameEntry?> Update(string originalName, NameEntry newEntry)
    {
        if (originalName != newEntry.Name)
        {
            // TODO: Fire an event that the name has changed
        }

        var filter = Builders<NameEntry>.Filter.Eq(ne => ne.Name, originalName);
        var updateStatement = GenerateUpdateStatement(newEntry);

        var options = new FindOneAndUpdateOptions<NameEntry>
        {
            ReturnDocument = ReturnDocument.After
        };

        var updated = await _nameEntryCollection.FindOneAndUpdateAsync(filter, updateStatement, options);

        if (updated == null)
        {
            // TODO: Fire an event that an attempt was made to update a non-existing name.
        }

        return updated;
    }

    public async Task<int> CountWhere(Expression<Func<NameEntry, bool>> filter)
    {
        var count = await _nameEntryCollection.CountDocumentsAsync(filter);
        return (int)count;
    }

    public async Task<List<NameEntry>> List(int pageNumber, int pageSize, Expression<Func<NameEntry, bool>>? filter = null)
    {
        var skipCount = (pageNumber - 1) * pageSize;
        var sort = Builders<NameEntry>.Sort.Ascending(ne => ne.Id);
        var query = _nameEntryCollection.Find(filter);

        var pagedEntries = await query
            .Sort(sort)
            .Skip(skipCount)
            .Limit(pageSize)
            .ToListAsync();

        return pagedEntries;
    }

    public async Task<NamesMetadataDto> GetMetadata()
    {
        return new NamesMetadataDto
        {
            TotalNames = await _nameEntryCollection.CountDocumentsAsync(FilterDefinition<NameEntry>.Empty),
            TotalNewNames = await _nameEntryCollection.CountDocumentsAsync(Builders<NameEntry>.Filter.Eq(x => x.State, State.NEW)),
            TotalModifiedNames = await _nameEntryCollection.CountDocumentsAsync(Builders<NameEntry>.Filter.Eq(x => x.State, State.MODIFIED)),
            TotalPublishedNames = await _nameEntryCollection.CountDocumentsAsync(Builders<NameEntry>.Filter.Eq(x => x.State, State.PUBLISHED))
        };
    }

    private static UpdateDefinition<NameEntry> GenerateUpdateStatement(NameEntry newEntry)
    {
        var statement = Builders<NameEntry>.Update
                    .Set(ne => ne.Name, newEntry.Name)
                    .Set(ne => ne.State, newEntry.State)
                    .Set(ne => ne.Pronunciation, newEntry.Pronunciation)
                    .Set(ne => ne.IpaNotation, newEntry.IpaNotation)
                    .Set(ne => ne.Meaning, newEntry.Meaning)
                    .Set(ne => ne.ExtendedMeaning, newEntry.ExtendedMeaning)
                    .Set(ne => ne.Morphology, newEntry.Morphology)
                    .Set(ne => ne.Media, newEntry.Media)
                    .Set(ne => ne.State, newEntry.State)
                    .Set(ne => ne.Etymology, newEntry.Etymology)
                    .Set(ne => ne.Videos, newEntry.Videos)
                    .Set(ne => ne.GeoLocation, newEntry.GeoLocation)
                    .Set(ne => ne.FamousPeople, newEntry.FamousPeople)
                    .Set(ne => ne.Syllables, newEntry.Syllables)
                    .Set(ne => ne.Variants, newEntry.Variants)
                    .Set(ne => ne.Modified, newEntry.Modified)
                    .Set(ne => ne.Duplicates, newEntry.Duplicates)
                    .Set(ne => ne.Feedbacks, newEntry.Feedbacks)
                    .CurrentDate(ne => ne.UpdatedAt);

        if (!string.IsNullOrWhiteSpace(newEntry.UpdatedBy))
        {
            statement.Set(ne => ne.UpdatedBy, newEntry.UpdatedBy);
        }

        return statement;
    }
}