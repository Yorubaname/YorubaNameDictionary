using Core.Repositories;
using YorubaOrganization.Core.Dto.Response;
using YorubaOrganization.Core.Repositories;

namespace Application.Services
{
    public class NameEntryFeedbackService
    {
        private readonly INameEntryFeedbackRepository _nameEntryFeedbackRepository;

        public NameEntryFeedbackService(INameEntryFeedbackRepository nameEntryFeedbackRepository)
        {
            _nameEntryFeedbackRepository = nameEntryFeedbackRepository;
        }

        public async Task<List<NameFeedbackDto>> FindAllAsync()
        {
            return await _nameEntryFeedbackRepository.FindAllAsync();
        }

        public async Task<List<NameFeedbackDto>> FindByNameAsync(string name)
        {
            return await _nameEntryFeedbackRepository.FindByNameAsync(name);
        }

        public async Task AddFeedbackByNameAsync(string name, string feedbackContent)
        {
            await _nameEntryFeedbackRepository.AddFeedbackByNameAsync(name, feedbackContent);
        }

        public async Task DeleteAllFeedbackForNameAsync(string name)
        {
            await _nameEntryFeedbackRepository.DeleteAllFeedbackForNameAsync(name);
        }

        public async Task<NameFeedbackDto> GetFeedbackByIdAsync(string feedbackId)
        {
            return await _nameEntryFeedbackRepository.GetFeedbackByIdAsync(feedbackId);
        }

        public async Task<bool> DeleteFeedbackAsync(string name, string id)
        {
            return await _nameEntryFeedbackRepository.DeleteFeedbackAsync(name, id);
        }
    }
}
