using Core.Dto.Response;
using Core.Entities.NameEntry.Collections;
namespace Core.Repositories
{
    public interface INameEntryFeedbackRepository
    {
        Task<List<FeedbackDto>> FindAllAsync();
        Task<List<FeedbackDto>> FindByNameAsync(string name);
        Task AddFeedbackByNameAsync(string name, string feedbackContent);
        Task DeleteAllFeedbackForNameAsync(string name);
        Task<FeedbackDto> GetFeedbackByIdAsync(string feedbackId);
        Task<bool> DeleteFeedbackAsync(string name, string feedbackId);
    }
}
