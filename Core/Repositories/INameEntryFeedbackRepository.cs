using Core.Entities.NameEntry.Collections;
namespace Core.Repositories
{
    public interface INameEntryFeedbackRepository
    {
        Task<List<Feedback>> FindAll(string sort);
        Task<List<Feedback>> FindByName(string name, string sort);
    }
}
