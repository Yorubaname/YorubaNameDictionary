using Core.Repositories.Words;
using Words.Core.Dto.Response;
using Words.Core.Entities;
using YorubaOrganization.Application.Services;
using YorubaOrganization.Core.Enums;

namespace Application.Services.Words
{
    public class WordSearchService(IWordEntryRepository wordsRepository) : SearchService<WordEntry>(wordsRepository)
    {
        /// <summary>
        /// Return number of published words
        /// </summary>
        /// <returns></returns>
        public async Task<WordsMetadataDto> GetWordsMetadata()
        {
            return new WordsMetadataDto(0, 0, 0, TotalPublishedWords: await GetTotalEntriesPublished());
        }

        public async Task<IEnumerable<WordEntry>> Search(string searchTerm)
        {
            var wordEntryRepository = (IWordEntryRepository)EntryRepository;
            var exactFound = await EntryRepository.FindByTitleAndState(searchTerm, State.PUBLISHED);
            if (exactFound != null)
            {
                return new List<WordEntry> { exactFound };
            }

            var startingWithSearchTerm = await EntryRepository.FindByTitleStartingWithAndState(searchTerm, State.PUBLISHED);
            if (startingWithSearchTerm.Count != 0)
            {
                return startingWithSearchTerm;
            }

            var possibleFound = new HashSet<WordEntry>();
            possibleFound.UnionWith(await EntryRepository.FindEntryByTitleContainingAndState(searchTerm, State.PUBLISHED));
            possibleFound.UnionWith(await EntryRepository.FindEntryByVariantsContainingAndState(searchTerm, State.PUBLISHED));
            // TODO YDict: Search the other fields of the word too, like Definitions and Examples.

            return possibleFound;
        }
    }
}