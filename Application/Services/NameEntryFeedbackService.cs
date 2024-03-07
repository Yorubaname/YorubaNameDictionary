using Core.Entities.NameEntry.Collections;
using Core.Entities.NameEntry;
using Core.Repositories;

namespace Application.Services
{
    public class NameEntryFeedbackService
    {
        private readonly INameEntryFeedbackRepository _nameEntryFeedbackRepository;

        public NameEntryFeedbackService(INameEntryFeedbackRepository nameEntryFeedbackRepository)
        {
            _nameEntryFeedbackRepository = nameEntryFeedbackRepository;
        }

        public async Task<List<Feedback>> FindAll(string sort)
        {
            return await _nameEntryFeedbackRepository.FindAll(sort);
        }
        public async Task<List<Feedback>> FindByName(string name, string sortOrder)
        {
            return await _nameEntryFeedbackRepository.FindByName(name, sortOrder);
        }
    }
}
