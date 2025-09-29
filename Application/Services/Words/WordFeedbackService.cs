using Words.Core.Dto.Response;
using Words.Core.Entities;
using YorubaOrganization.Application.Services;

namespace Application.Services.Words
{
    public class WordFeedbackService(EntryFeedbackService<WordEntry> entryFeedbackService)
    {
        public async Task<List<WordFeedbackDto>> FindAllAsync()
        {
            var result = await entryFeedbackService.FindAllAsync();
            return result.Select(f => new WordFeedbackDto(f.Id, f.Title, f.Feedback, f.SubmittedAt)).ToList();
        }

        public async Task<List<WordFeedbackDto>> FindByWordAsync(string word)
        {
            var result = await entryFeedbackService.FindByTitleAsync(word);
            return result.Select(f => new WordFeedbackDto(f.Id, f.Title, f.Feedback, f.SubmittedAt)).ToList();
        }

        public async Task AddFeedbackForWordAsync(string word, string feedbackContent)
        {
            await entryFeedbackService.AddFeedbackToEntryAsync(word, feedbackContent);
        }

        public async Task DeleteAllFeedbackForWordAsync(string word)
        {
            await entryFeedbackService.DeleteAllEntryFeedbackAsync(word);
        }

        public async Task<WordFeedbackDto?> GetFeedbackByIdAsync(string feedbackId)
        {
            var result = await entryFeedbackService.GetFeedbackByIdAsync(feedbackId);
            return result == null ? null : new WordFeedbackDto(result.Id, result.Title, result.Feedback, result.SubmittedAt);
        }

        public async Task<bool> DeleteFeedbackAsync(string word, string id)
        {
            return await entryFeedbackService.DeleteFeedbackAsync(word, id);
        }
    }
}
