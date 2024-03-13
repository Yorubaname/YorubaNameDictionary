using Core.Entities.NameEntry;
using Core.Entities.NameEntry.Collections;
using Core.Repositories;
using MongoDB.Bson;
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
                   
        var nameEntryAndFeedbacks = await _nameEntryCollection
            .Find(_ => true)
            .Sort(sortDefinition)
            .ToListAsync();

        var feedbacks = nameEntryAndFeedbacks.SelectMany(x => x.Feedbacks).ToList();
        
        return feedbacks;
    }

    public async Task<List<Feedback>> FindByNameAsync(string name, string sortOrder)
    {
        var sortDefinition = sortOrder.Equals("desc", System.StringComparison.OrdinalIgnoreCase)
            ? Builders<NameEntry>.Sort.Descending("Feedbacks.CreatedAt")
            : Builders<NameEntry>.Sort.Ascending("Feedbacks.CreatedAt");

        var data = await _nameEntryCollection
            .Find(entry => entry.Name == name)
            .Sort(sortDefinition)
            .ToListAsync();

        return data.SelectMany(x => x.Feedbacks).ToList();
    }

    public async Task<bool> AddFeedbackByNameAsync(string name, string feedbackContent)
    {
        var filter = Builders<NameEntry>.Filter
            .Eq(entry => entry.Name, name);

        var nameFeedback = new Feedback
        {
            Content = feedbackContent,
            Id = ObjectId.GenerateNewId().ToString()
        };

        var update = Builders<NameEntry>.Update
            .Push(entry => entry.Feedbacks, nameFeedback);

        var updateResult = await _nameEntryCollection.UpdateOneAsync(filter, update);

        return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAllFeedbackForNameAsync(string name)
    {
        var filter = Builders<NameEntry>.Filter.Eq(entry => entry.Name, name);
        var update = Builders<NameEntry>.Update.Set(entry => entry.Feedbacks, new List<Feedback>());

        var updateResult = await _nameEntryCollection.UpdateOneAsync(filter, update);

        return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }

    public async Task<bool> DeleteFeedbackAsync(string name, string content)
    {
        var filter = Builders<NameEntry>.Filter.Eq(entry => entry.Name, name) &
                     Builders<NameEntry>.Filter.ElemMatch(entry => entry.Feedbacks, feedback => feedback.Content == content);

        var update = Builders<NameEntry>.Update.PullFilter(entry => entry.Feedbacks, feedback => feedback.Content == content);

        var updateResult = await _nameEntryCollection.UpdateOneAsync(filter, update);

        return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }

    public async Task<Feedback> GetFeedbackByIdAsync(string feedbackId)
    {
        var filter = Builders<NameEntry>.Filter.ElemMatch(entry => entry.Feedbacks, feedback => feedback.Id == feedbackId);
        var projectionDefinition = Builders<NameEntry>.Projection.ElemMatch(entry => entry.Feedbacks, feedback => feedback.Id == feedbackId);

        var nameEntry = await _nameEntryCollection.Find(filter)
            .Project<NameEntry>(projectionDefinition)
            .FirstOrDefaultAsync();

        return nameEntry?.Feedbacks?.FirstOrDefault(feedback => feedback.Id == feedbackId);
    }

    public async Task<bool> DeleteFeedbackByIdAsync(string feedbackId)
    {
        var filter = Builders<NameEntry>.Filter.ElemMatch(entry => entry.Feedbacks, feedback => feedback.Id == feedbackId);
        var update = Builders<NameEntry>.Update.PullFilter(entry => entry.Feedbacks, feedback => feedback.Id == feedbackId);

        var existingEntry = await _nameEntryCollection.Find(filter).FirstOrDefaultAsync();
        if (existingEntry == null || existingEntry.Feedbacks == null || !existingEntry.Feedbacks.Any())
        {
            // If the entry or its Feedbacks collection is null or empty, the feedback with the specified ID does not exist.
            return false;
        }

        var updateResult = await _nameEntryCollection.UpdateOneAsync(filter, update);

        return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }
}
