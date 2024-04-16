using Core.Entities.NameEntry.Collections;
namespace Core.Repositories
{
    public interface INameEntryFeedbackRepository
    {
        Task<List<Feedback>> FindAllAsync();
        Task<List<Feedback>> FindByNameAsync(string name);
        Task AddFeedbackByNameAsync(string name, string feedbackContent);
        Task<bool> DeleteAllFeedbackForNameAsync(string name);
        Task<Feedback> GetFeedbackByIdAsync(string feedbackId);
        Task<bool> DeleteFeedbackAsync(string name, string feedbackId);
    }
}
