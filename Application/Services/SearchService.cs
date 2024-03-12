using Core.Dto;
using Core.Repositories;

namespace Application.Services
{
    public class SearchService
    {
        private readonly INameEntryRepository _namesRepository;

        public SearchService(INameEntryRepository namesRepository) 
        {
            _namesRepository = namesRepository;
        }

        public async Task<SearchMetadataDto> GetNamesMetadata()
        {
            // Return number of published names
            var totalPublishedNames = await _namesRepository.CountByState(Core.Enums.State.PUBLISHED);
            return new SearchMetadataDto { TotalPublishedNames = totalPublishedNames };
        }
    }
}
