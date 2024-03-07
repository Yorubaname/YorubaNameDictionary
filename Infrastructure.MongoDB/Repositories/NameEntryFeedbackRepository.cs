using Core.Entities.NameEntry;
using Core.Entities.NameEntry.Collections;
using Core.Repositories;
using MongoDB.Driver;

namespace Infrastructure.MongoDB.Repositories;

public class NameEntryFeedbackRepository : INameEntryFeedbackRepository
{
    private readonly IMongoCollection<NameEntry> _nameEntryCollection;

    public NameEntryFeedbackRepository(IMongoDatabase database)
    {
        _nameEntryCollection = database.GetCollection<NameEntry>("NameEntries");
    }

    public async Task<List<Feedback>> FindAll(string sortOrder)
    {
        var sortDefinition = sortOrder.Equals("desc", System.StringComparison.OrdinalIgnoreCase)
            ? Builders<NameEntry>.Sort.Descending("Feedbacks.CreatedAt")
            : Builders<NameEntry>.Sort.Ascending("Feedbacks.CreatedAt");

        var projectionDefinition = Builders<NameEntry>.Projection.Include("Feedbacks.Content").Exclude("_id");

        return await _nameEntryCollection
            .Find(_ => true)
            .Sort(sortDefinition)
            .Project<Feedback>(projectionDefinition)
            .ToListAsync();
    }

    public async Task<List<Feedback>> FindByName(string name, string sortOrder)
    {
        var sortDefinition = sortOrder.Equals("desc", System.StringComparison.OrdinalIgnoreCase)
            ? Builders<NameEntry>.Sort.Descending("Feedbacks.CreatedAt")
            : Builders<NameEntry>.Sort.Ascending("Feedbacks.CreatedAt");

        var projectionDefinition = Builders<NameEntry>.Projection.Include("Feedbacks.Content").Exclude("_id");

        return await _nameEntryCollection
            .Find(entry => entry.Name == name)
            .Sort(sortDefinition)
            .Project<Feedback>(projectionDefinition)
            .ToListAsync();
    }
}
