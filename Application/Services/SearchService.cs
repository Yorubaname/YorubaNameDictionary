using Core.Dto.Response;
using Core.Entities.NameEntry;
using Core.Enums;
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

        public async Task<IEnumerable<NameEntry>> Search(string searchTerm)
        {
            var exactFound = await _namesRepository.FindByNameAndState(searchTerm, State.PUBLISHED);
            if (exactFound != null)
            {
                return new NameEntry[] { exactFound };
            }

            var startingWithSearchTerm = await _namesRepository.FindByNameStartingWithAndState(searchTerm, State.PUBLISHED);
            if (startingWithSearchTerm.Any())
            {
                return startingWithSearchTerm;
            }

            var possibleFound = new HashSet<NameEntry>();
            possibleFound.UnionWith(await _namesRepository.FindNameEntryByNameContainingAndState(searchTerm, State.PUBLISHED));
            possibleFound.UnionWith(await _namesRepository.FindNameEntryByVariantsContainingAndState(searchTerm, State.PUBLISHED));
            possibleFound.UnionWith(await _namesRepository.FindNameEntryByMeaningContainingAndState(searchTerm, State.PUBLISHED));
            possibleFound.UnionWith(await _namesRepository.FindNameEntryByExtendedMeaningContainingAndState(searchTerm, State.PUBLISHED));

            return possibleFound;

        }
    }
}
