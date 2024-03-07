using Core.Entities.NameEntry.Collections;
namespace Core.Repositories
{
    public interface INameEntryFeedbackRepository
    {
        Task<List<Feedback>> FindAllAsync(string sort);
        Task<List<Feedback>> FindByNameAsync(string name, string sort);
        Task<bool> AddFeedbackByNameAsync(string name, string feedbackContent);
    }
}
