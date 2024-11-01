using Core.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using YorubaOrganization.Core.Dto.Response;
using YorubaOrganization.Core.Entities;
using YorubaOrganization.Core.Enums;
using YorubaOrganization.Core.Events;
using YorubaOrganization.Core.Repositories;

namespace Infrastructure.MongoDB.Repositories;

public abstract class DictionaryEntryRepository<TEntity> : MongoDBRepository, IDictionaryEntryRepository<TEntity> where TEntity : DictionaryEntry<TEntity>
{
    protected readonly IMongoCollection<TEntity> EntryCollection;
    private readonly IEventPubService _eventPubService;

    public DictionaryEntryRepository(
        string collectionName,
        IMongoDatabase database,
        IEventPubService eventPubService)
    {
        EntryCollection = database.GetCollection<TEntity>(collectionName);
        _eventPubService = eventPubService;

        CreateIndexes(collectionName);
    }

    private void CreateIndexes(string collectionName)
    {
        var indexKeys = Builders<TEntity>.IndexKeys.Ascending(x => x.Title);
        var indexOptions = new CreateIndexOptions
        {
            Unique = true,
            Name = $"IX_{collectionName}_Title_Unique",
            Background = true
        };
        EntryCollection.Indexes.CreateOne(new CreateIndexModel<TEntity>(indexKeys, indexOptions));
    }

    public async Task<TEntity> FindById(string id)
    {
        return await EntryCollection.Find(x => x.Id == id).SingleOrDefaultAsync();
    }

    public async Task<bool> DeleteAll()
    {
        var deleteResult = await EntryCollection.DeleteManyAsync(FilterDefinition<TEntity>.Empty);
        return deleteResult.DeletedCount > 0;
    }


    public async Task Create(TEntity entry)
    {
        entry.Id = ObjectId.GenerateNewId().ToString();
        await EntryCollection.InsertOneAsync(entry);
    }

    public async Task Create(List<TEntity> entries)
    {
        entries.ForEach(entry => entry.Id = ObjectId.GenerateNewId().ToString()!);
        await EntryCollection.InsertManyAsync(entries);
    }

    public async Task<int> CountByState(State state)
    {
        return await CountWhere(ne => ne.State == state);
    }

    public async Task Delete(string title)
    {
        var filter = Builders<TEntity>.Filter.Eq(e => e.Title, title);
        var options = SetCollationPrimary<DeleteOptions>(new DeleteOptions());

        await EntryCollection.DeleteOneAsync(filter, options);
    }
    public async Task DeleteMany(string[] titles)
    {
        var filter = Builders<TEntity>.Filter.In(e => e.Title, titles);
        var options = SetCollationPrimary<DeleteOptions>(new DeleteOptions());

        await EntryCollection.DeleteManyAsync(filter, options);
    }

    public async Task<bool> DeleteByTitleAndState(string title, State state)
    {
        var filter = Builders<TEntity>
            .Filter
            .And(Builders<TEntity>.Filter.Eq(e => e.Title, title), Builders<TEntity>.Filter.Eq("State", state));

        var options = SetCollationPrimary<DeleteOptions>(new DeleteOptions());

        var deleteResult = await EntryCollection.DeleteOneAsync(filter, options);

        return deleteResult.DeletedCount > 0;
    }

    // TODO Hafiz (Later): This is pulling too much data. We should eventually get rid of it.
    public async Task<HashSet<TEntity>> ListAll()
    {
        var allEntries = await EntryCollection.Find(_ => true).ToListAsync();
        return new HashSet<TEntity>(allEntries);
    }

    public async Task<TEntity?> FindByTitle(string title)
    {
        var filter = Builders<TEntity>.Filter.Eq(e => e.Title, title);
        var options = SetCollationPrimary<FindOptions>(new FindOptions());

        return await EntryCollection.Find(filter, options).SingleOrDefaultAsync();
    }

    public async Task<List<TEntity>> FindByTitles(string[] titles)
    {
        var filter = Builders<TEntity>.Filter.In(e => e.Title, titles);
        var options = SetCollationPrimary<FindOptions>(new FindOptions());

        return await EntryCollection.Find(filter, options).ToListAsync();
    }

    public async Task<TEntity?> FindByTitleAndState(string title, State state)
    {
        var options = SetCollationPrimary<FindOptions>(new FindOptions());
        return await EntryCollection
                            .Find(ne => ne.Title == title && ne.State == state, options)
                            .SingleOrDefaultAsync();
    }

    public async Task<HashSet<TEntity>> FindByTitleStartingWithAndState(string searchTerm, State state)
    {
        return await FindByTitleStartingWithAnyAndState([searchTerm], state);
    }

    public async Task<HashSet<TEntity>> FindByTitleStartingWithAnyAndState(IEnumerable<string> searchTerms, State state)
    {
        var regexFilters = searchTerms.Select(term =>
        {
            return Builders<TEntity>.Filter.Regex(ne => ne.Title,
                new BsonRegularExpression($"^{term.ReplaceYorubaVowelsWithPattern()}", "i"));
        });

        var entryTitleFilter = Builders<TEntity>.Filter.Or(regexFilters);
        var stateFilter = Builders<TEntity>.Filter.Eq(ne => ne.State, state);
        var combinedFilter = Builders<TEntity>.Filter.And(entryTitleFilter, stateFilter);

        var result = await EntryCollection.Find(combinedFilter).ToListAsync();
        return new HashSet<TEntity>(result);
    }


    public async Task<List<TEntity>> FindByState(State state)
    {
        return await EntryCollection.Find(ne => ne.State == state).ToListAsync();
    }

    public async Task<HashSet<TEntity>> FindEntryByMeaningContainingAndState(string title, State state)
    {
        var filter = Builders<TEntity>.Filter.Regex(ne => ne.Title,
            new BsonRegularExpression(title.ReplaceYorubaVowelsWithPattern(), "i"))
                                    & Builders<TEntity>.Filter.Eq(ne => ne.State, state);
        var result = await EntryCollection.Find(filter).ToListAsync();
        return new HashSet<TEntity>(result);
    }

    public async Task<HashSet<TEntity>> FindEntryByTitleContainingAndState(string title, State state)
    {
        var filter = Builders<TEntity>.Filter.Regex(ne => ne.Title,
            new BsonRegularExpression(title.ReplaceYorubaVowelsWithPattern(), "i")) &
             Builders<TEntity>.Filter.Eq(ne => ne.State, state);
        var result = await EntryCollection.Find(filter).ToListAsync();
        return new HashSet<TEntity>(result);
    }

    public async Task<HashSet<TEntity>> FindEntryByVariantsContainingAndState(string title, State state)
    {
        var regex = new BsonRegularExpression(title.ReplaceYorubaVowelsWithPattern(), "i");
        var filter = Builders<TEntity>.Filter.Regex(ne => ne.VariantsV2, regex) & Builders<TEntity>.Filter.Eq(ne => ne.State, state);
        var result = await EntryCollection.Find(filter).ToListAsync();
        return new HashSet<TEntity>(result);
    }

    public async Task<TEntity?> Update(string originalTitle, TEntity newEntry)
    {
        var filter = Builders<TEntity>.Filter.Eq(ne => ne.Title, originalTitle);
        var updateStatement = GenerateUpdateStatement(newEntry);

        var options = new FindOneAndUpdateOptions<TEntity>
        {
            ReturnDocument = ReturnDocument.After
        };

        var updated = await EntryCollection.FindOneAndUpdateAsync(filter, updateStatement, options);

        if (updated == null)
        {
            await _eventPubService.PublishEvent(new NonExistingEntryUpdateAttempted(originalTitle));
        }
        else if (originalTitle != newEntry.Title)
        {
            await _eventPubService.PublishEvent(new EntryTitleUpdated(originalTitle, newEntry.Title));
        }

        return updated;
    }

    public async Task<int> CountWhere(Expression<Func<TEntity, bool>> filter)
    {
        var count = await EntryCollection.CountDocumentsAsync(filter);
        return (int)count;
    }

    public async Task<IEnumerable<TEntity>> GetAllEntries(State? state, string? submittedBy)
    {
        var filterBuilder = Builders<TEntity>.Filter;
        var filter = filterBuilder.Empty;

        if (state.HasValue)
        {
            filter &= filterBuilder.Eq(ne => ne.State, state.Value);
        }

        if (!string.IsNullOrWhiteSpace(submittedBy))
        {
            filter &= filterBuilder.Eq(ne => ne.CreatedBy, submittedBy.Trim());
        }

        var projection = Builders<TEntity>.Projection.Include(ne => ne.Title).Exclude(ne => ne.Id);

        return await EntryCollection
                            .Find(filter)
                            .Project<TEntity>(projection)
                            .Sort(Builders<TEntity>.Sort.Ascending(ne => ne.CreatedAt))
                            .ToListAsync();
    }

    public async Task<List<TEntity>> List(int pageNumber, int pageSize, State? state, string? submittedBy)
    {
        var filterBuilder = Builders<TEntity>.Filter;
        var filter = filterBuilder.Empty;

        if (state.HasValue)
        {
            filter &= filterBuilder.Eq(ne => ne.State, state.Value);
        }

        if (!string.IsNullOrWhiteSpace(submittedBy))
        {
            filter &= filterBuilder.Eq(ne => ne.CreatedBy, submittedBy.Trim());
        }

        int skip = (pageNumber - 1) * pageSize;
        return await EntryCollection
                            .Find(filter)
                            .Skip(skip)
                            .Limit(pageSize)
                            .Sort(Builders<TEntity>.Sort.Ascending(ne => ne.CreatedAt))
                            .ToListAsync();
    }

    public async Task<MetadataResponse> GetMetadata()
    {
        return new MetadataResponse
        {
            TotalEntries = await EntryCollection.CountDocumentsAsync(FilterDefinition<TEntity>.Empty),
            TotalNewEntries = await EntryCollection.CountDocumentsAsync(Builders<TEntity>.Filter.Eq(x => x.State, State.NEW)),
            TotalModifiedEntries = await EntryCollection.CountDocumentsAsync(Builders<TEntity>.Filter.Eq(x => x.State, State.MODIFIED)),
            TotalPublishedEntries = await EntryCollection.CountDocumentsAsync(Builders<TEntity>.Filter.Eq(x => x.State, State.PUBLISHED))
        };
    }

    private UpdateDefinition<TEntity> GenerateUpdateStatement(TEntity newEntry)
    {
        var statement = GenerateCustomUpdateStatement(newEntry)
                    .Set(ne => ne.Title, newEntry.Title)
                    .Set(ne => ne.State, newEntry.State)
                    .Set(ne => ne.Pronunciation, newEntry.Pronunciation)
                    .Set(ne => ne.IpaNotation, newEntry.IpaNotation)
                    .Set(ne => ne.Morphology, newEntry.Morphology)
                    .Set(ne => ne.MediaLinks, newEntry.MediaLinks)
                    .Set(ne => ne.State, newEntry.State)
                    .Set(ne => ne.Etymology, newEntry.Etymology)
                    .Set(ne => ne.Videos, newEntry.Videos)
                    .Set(ne => ne.GeoLocation, newEntry.GeoLocation)
                    .Set(ne => ne.Syllables, newEntry.Syllables)
                    .Set(ne => ne.VariantsV2, newEntry.VariantsV2)
                    .Set(ne => ne.Modified, newEntry.Modified)
                    .CurrentDate(ne => ne.UpdatedAt);

        if (!string.IsNullOrWhiteSpace(newEntry.UpdatedBy))
        {
            statement = statement.Set(ne => ne.UpdatedBy, newEntry.UpdatedBy);
        }

        if (newEntry.Duplicates.Any())
        {
            statement = statement.Set(ne => ne.Duplicates, newEntry.Duplicates);
        }

        return statement;
    }

    protected abstract UpdateDefinition<TEntity> GenerateCustomUpdateStatement(TEntity newEntry);
}