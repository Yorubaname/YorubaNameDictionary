using Core.Dto.Response;
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

    public async Task<List<FeedbackDto>> FindAllAsync()
    {
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$unwind", "$Feedbacks"),
            new BsonDocument("$project", new BsonDocument
            {
                { "Name", "$Name" },
                { "Feedback", "$Feedbacks.Content" },
                { "SubmittedAt", "$Feedbacks.CreatedAt" },
                { "_id", "$Feedbacks._id" }
            }),
            new BsonDocument("$sort", new BsonDocument("SubmittedAt", -1))
        };

        var feedbacks = await _nameEntryCollection
            .Aggregate<FeedbackDto>(pipeline)
            .ToListAsync();

        return feedbacks;
    }

    public async Task<List<FeedbackDto>> FindByNameAsync(string name)
    {
        var theName = await _nameEntryCollection
            .Find(entry => entry.Title.ToLower() == name.ToLower())
            .SingleOrDefaultAsync();

        var feedbacksForName = theName?.Feedbacks?.Select(f => new FeedbackDto(f.Id, theName.Title, f.Content!, f.CreatedAt))
            .OrderByDescending(x => x.SubmittedAt)
            .ToList();

        return feedbacksForName ?? new List<FeedbackDto>();
    }

    public async Task AddFeedbackByNameAsync(string name, string feedbackContent)
    {
        var filter = Builders<NameEntry>.Filter.Where(x => x.Title.ToLower() == name.ToLower());

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
        var filter = Builders<NameEntry>.Filter.Where(x => x.Title.ToLower() == name.ToLower());
        var update = Builders<NameEntry>.Update.Set(entry => entry.Feedbacks, new List<Feedback>());

        await _nameEntryCollection.UpdateOneAsync(filter, update);
    }

    public async Task<bool> DeleteFeedbackAsync(string name, string feedbackId)
    {
        var filter = Builders<NameEntry>.Filter.Where(x => x.Title.ToLower() == name.ToLower());
        var entry = await _nameEntryCollection.Find(filter).FirstOrDefaultAsync();

        if (entry != null && entry.Feedbacks.Any(feedback => feedback.Id == feedbackId))
        {
            var update = Builders<NameEntry>.Update.PullFilter(e => e.Feedbacks, feedback => feedback.Id == feedbackId);
            var result = await _nameEntryCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        return false;
    }

    public async Task<FeedbackDto?> GetFeedbackByIdAsync(string feedbackId)
    {
        var filter = Builders<NameEntry>.Filter.ElemMatch(entry => entry.Feedbacks, feedback => feedback.Id == feedbackId);
        var projectionDefinition = Builders<NameEntry>
            .Projection
            .Include(entry => entry.Title)
            .ElemMatch(entry => entry.Feedbacks, feedback => feedback.Id == feedbackId);

        var nameEntry = await _nameEntryCollection.Find(filter)
            .Project<NameEntry>(projectionDefinition)
            .FirstOrDefaultAsync();

        var theMatch = nameEntry?.Feedbacks?.FirstOrDefault(feedback => feedback.Id == feedbackId);
        return theMatch == null ? null : new FeedbackDto(theMatch.Id, nameEntry!.Title, theMatch!.Content!, theMatch.CreatedAt);
    }

}
