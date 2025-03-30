using Core.Entities;
using Core.Repositories;
using YorubaOrganization.Application.Services;
using YorubaOrganization.Core.Dto.Response;
using YorubaOrganization.Core.Enums;

namespace Application.Services.Names
{
    public class NameSearchService(INameEntryRepository namesRepository) : SearchService<NameEntry>(namesRepository)
    {
        /// <summary>
        /// Return number of published names
        /// </summary>
        /// <returns></returns>
        public async Task<SearchMetadataDto> GetNamesMetadata()
        {
            return new SearchMetadataDto { TotalPublishedNames = await GetTotalEntriesPublished() };
        }

        public async Task<IEnumerable<NameEntry>> Search(string searchTerm)
        {
            var nameEntryRepository = (INameEntryRepository)EntryRepository;
            var exactFound = await EntryRepository.FindByTitleAndState(searchTerm, State.PUBLISHED);
            if (exactFound != null)
            {
                return [exactFound];
            }

            var startingWithSearchTerm = await EntryRepository.FindByTitleStartingWithAndState(searchTerm, State.PUBLISHED);
            if (startingWithSearchTerm.Count != 0)
            {
                return startingWithSearchTerm;
            }

            var possibleFound = new HashSet<NameEntry>();
            possibleFound.UnionWith(await EntryRepository.FindEntryByTitleContainingAndState(searchTerm, State.PUBLISHED));
            possibleFound.UnionWith(await EntryRepository.FindEntryByVariantsContainingAndState(searchTerm, State.PUBLISHED));
            possibleFound.UnionWith(await nameEntryRepository.FindEntryByMeaningContainingAndState(searchTerm, State.PUBLISHED));
            possibleFound.UnionWith(await nameEntryRepository.FindEntryByExtendedMeaningContainingAndState(searchTerm, State.PUBLISHED));

            return possibleFound;

        }
    }
}
