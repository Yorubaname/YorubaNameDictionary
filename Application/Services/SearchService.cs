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

        public async Task<HashSet<string>> AutoComplete(string query)
        {
            var searchTerms = new List<string>();

            // TODO: Calling the db in a for loop might not be a terribly good idea. Revisit
            for (int i = 2; i <= query.Length; i++)
            {
                searchTerms.Add(query.Substring(0, i));
            }

            var namesResult = await _namesRepository.FindByNameStartingWithAnyAndState(searchTerms, State.PUBLISHED);
            var namesContainingQuery = await _namesRepository.FindNameEntryByNameContainingAndState(query, State.PUBLISHED);
            namesResult.UnionWith(namesContainingQuery);

            return new HashSet<string>(namesResult.Select(n => n.Name));
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
