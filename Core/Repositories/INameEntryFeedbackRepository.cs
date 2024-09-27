using YorubaOrganization.Core.Dto.Response;

namespace Core.Repositories
{
    public interface INameEntryFeedbackRepository
    {
        Task<List<NameFeedbackDto>> FindAllAsync();
        Task<List<NameFeedbackDto>> FindByNameAsync(string name);
        Task AddFeedbackByNameAsync(string name, string feedbackContent);
        Task DeleteAllFeedbackForNameAsync(string name);
        Task<NameFeedbackDto> GetFeedbackByIdAsync(string feedbackId);
        Task<bool> DeleteFeedbackAsync(string name, string feedbackId);
    }
}
