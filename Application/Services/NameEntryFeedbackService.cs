using Core.Dto.Response;
using YorubaOrganization.Application.Services;

namespace Application.Services
{
    public class NameEntryFeedbackService(EntryFeedbackService entryFeedbackService)
    {
        public async Task<List<NameFeedbackDto>> FindAllAsync()
        {
            var result = await entryFeedbackService.FindAllAsync();
            return result.Select(f => new NameFeedbackDto(f.Id, f.Title, f.Feedback, f.SubmittedAt)).ToList();
        }

        public async Task<List<NameFeedbackDto>> FindByNameAsync(string name)
        {
            var result = await entryFeedbackService.FindByTitleAsync(name);
            return result.Select(f => new NameFeedbackDto(f.Id, f.Title, f.Feedback, f.SubmittedAt)).ToList();
        }

        public async Task AddFeedbackByNameAsync(string name, string feedbackContent)
        {
            await entryFeedbackService.AddFeedbackToEntryAsync(name, feedbackContent);
        }

        public async Task DeleteAllFeedbackForNameAsync(string name)
        {
            await entryFeedbackService.DeleteAllEntryFeedbackAsync(name);
        }

        public async Task<NameFeedbackDto?> GetFeedbackByIdAsync(string feedbackId)
        {
            var result = await entryFeedbackService.GetFeedbackByIdAsync(feedbackId);
            return result == null ? null : new NameFeedbackDto(result.Id, result.Title, result.Feedback, result.SubmittedAt);
        }

        public async Task<bool> DeleteFeedbackAsync(string name, string id)
        {
            return await entryFeedbackService.DeleteFeedbackAsync(name, id);
        }
    }
}
