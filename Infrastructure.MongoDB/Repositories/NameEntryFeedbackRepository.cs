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

    public async Task<List<Feedback>> FindAllAsync(string sortOrder)
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

    public async Task<List<Feedback>> FindByNameAsync(string name, string sortOrder)
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

    public async Task<bool> AddFeedbackByNameAsync(string name, string feedbackContent)
    {
        var filter = Builders<NameEntry>.Filter
            .Eq(entry => entry.Name, name);

        var update = Builders<NameEntry>.Update
            .Push(entry => entry.Feedbacks, new Feedback { Content = feedbackContent });

        var updateResult = await _nameEntryCollection.UpdateOneAsync(filter, update);

        return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }
}
