using Core.Entities.NameEntry.Collections;
namespace Core.Repositories
{
    public interface INameEntryFeedbackRepository
    {
        Task<List<Feedback>> FindAllAsync();
        Task<List<Feedback>> FindByNameAsync(string name);
        Task AddFeedbackByNameAsync(string name, string feedbackContent);
        Task DeleteAllFeedbackForNameAsync(string name);
        Task<Feedback> GetFeedbackByIdAsync(string feedbackId);
        Task DeleteFeedbackAsync(string name, string feedbackId);
    }
}
