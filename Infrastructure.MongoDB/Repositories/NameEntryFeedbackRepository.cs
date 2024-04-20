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

    public async Task<List<Feedback>> FindAllAsync()
    {     
        var nameEntryAndFeedbacks = await _nameEntryCollection
            .Find(_ => true)
            .ToListAsync();

        var feedbacks = nameEntryAndFeedbacks.SelectMany(x => x.Feedbacks)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
        
        return feedbacks;
    }

    public async Task<List<Feedback>> FindByNameAsync(string name)
    {
        var data = await _nameEntryCollection
            .Find(entry => entry.Name.ToLower() == name.ToLower())            
            .ToListAsync();

        var feedbacksForName = data.SelectMany(x => x.Feedbacks)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return feedbacksForName;
    }

    public async Task AddFeedbackByNameAsync(string name, string feedbackContent)
    {
        var filter = Builders<NameEntry>.Filter.Where(x => x.Name.ToLower() == name.ToLower());

        var nameFeedback = new Feedback
        {
            Content = feedbackContent,
            Id = ObjectId.GenerateNewId().ToString()
        };

        var update = Builders<NameEntry>.Update
            .Push(entry => entry.Feedbacks, nameFeedback);

        await _nameEntryCollection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteAllFeedbackForNameAsync(string name)
    {
        var filter = Builders<NameEntry>.Filter.Where(x => x.Name.ToLower() == name.ToLower());
        var update = Builders<NameEntry>.Update.Set(entry => entry.Feedbacks, new List<Feedback>());

        await _nameEntryCollection.UpdateOneAsync(filter, update);
    }

    public async Task<bool> DeleteFeedbackAsync(string name, string feedbackId)
    {
        var filter = Builders<NameEntry>.Filter.Where(x => x.Name.ToLower() == name.ToLower());
        var entry = await _nameEntryCollection.Find(filter).FirstOrDefaultAsync();

        if (entry != null && entry.Feedbacks.Any(feedback => feedback.Id == feedbackId))
        {
            var update = Builders<NameEntry>.Update.PullFilter(e => e.Feedbacks, feedback => feedback.Id == feedbackId);
            var result = await _nameEntryCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        return false;
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

}
