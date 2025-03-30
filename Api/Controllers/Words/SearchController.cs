using Api.Utilities;
using Application.Services.MultiLanguage;
using Application.Services.Words;
using Words.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using YorubaOrganization.Core.Cache;
using YorubaOrganization.Core.Dto.Response;
using YorubaOrganization.Core.Enums;
using YorubaOrganization.Core.Events;
using Words.Core.Dto.Response;
using Application.Mappers.Words;

namespace Api.Controllers.Words
{
    [Route("api/v1/dictionary/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private const string SearchActivity = "search", IndexActivity = "index", PopularActivity = "popular";
        private readonly WordSearchService searchService;
        private readonly IEventPubService eventPubService;
        private readonly IRecentSearchesCache recentSearchesCache;
        private readonly IRecentIndexesCache recentIndexesCache;
        private readonly WordEntryService wordEntryService;
        private readonly ILanguageService languageService;
        
        public SearchController(
            WordSearchService searchService,
            IEventPubService eventPubService,
            IRecentSearchesCache recentSearchesCache,
            IRecentIndexesCache recentIndexesCache,
            WordEntryService wordEntryService,
            ILanguageService languageService)
        {
            this.searchService = searchService;
            this.eventPubService = eventPubService;
            this.recentSearchesCache = recentSearchesCache;
            this.recentIndexesCache = recentIndexesCache;
            this.wordEntryService = wordEntryService;
            this.languageService = languageService;
        }

        [HttpGet("meta")]
        [ProducesResponseType(typeof(SearchMetadataDto[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMetadata()
        {
            var metadata = await searchService.GetWordsMetadata();
            return Ok(metadata);
        }

        [HttpGet]
        [ProducesResponseType(typeof(WordEntryDto[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Search([FromQuery(Name = "q"), Required] string searchTerm)
        {
            var matches = await searchService.Search(searchTerm);

            if (matches.Count() == 1 && matches.First().Title.Equals(searchTerm, StringComparison.CurrentCultureIgnoreCase))
            {
                await eventPubService.PublishEvent(new ExactEntrySearched(matches.First().Title, languageService.CurrentTenant));
            }

            return Ok(matches.MapToDtoCollection());
        }

        [HttpGet("autocomplete")]
        [ProducesResponseType(typeof(string[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Autocomplete([FromQuery(Name = "q")] string? searchTerm = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            {
                return Ok(Enumerable.Empty<string>());
            }

            return Ok(await searchService.AutoComplete(searchTerm));
        }

        [HttpGet("alphabet/{searchTerm}")]
        [ProducesResponseType(typeof(IEnumerable<WordEntryDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchByStartsWith(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Ok(Enumerable.Empty<WordEntryDto>());
            }

            var wordEntry = await searchService.SearchByStartsWith(searchTerm);
            return Ok(wordEntry.MapToDtoCollection());
        }

        [HttpGet("{searchTerm}")]
        [ProducesResponseType(typeof(WordEntryDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchOne(string searchTerm)
        {
            var wordEntry = (await searchService.GetEntry(searchTerm))?.MapToDto();

            if (wordEntry != null)
            {
                await eventPubService.PublishEvent(new ExactEntrySearched(wordEntry.Word, languageService.CurrentTenant));
            }

            return Ok(wordEntry);
        }

        [HttpGet("activity")]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetRecentStatsByActivity([FromQuery(Name = "q")] string? activityType = null)
        {
            if (string.IsNullOrEmpty(activityType))
            {
                return RedirectToAction(nameof(GetRecentStats));
            }

            if (activityType.Equals(SearchActivity, StringComparison.OrdinalIgnoreCase))
            {
                return Ok(await recentSearchesCache.Get());
            }

            if (activityType.Equals(IndexActivity, StringComparison.OrdinalIgnoreCase))
            {
                return Ok(await recentIndexesCache.Get());
            }

            if (activityType.Equals(PopularActivity, StringComparison.OrdinalIgnoreCase))
            {
                return Ok(await recentSearchesCache.GetMostPopular());
            }

            return BadRequest("Activity type not recognized");
        }

        [HttpGet("activity/all")]
        [ProducesResponseType(typeof(Dictionary<string, IEnumerable<string>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRecentStats()
        {
            var result = new Dictionary<string, IEnumerable<string>>()
            {
                { SearchActivity, await recentSearchesCache.Get() },
                { PopularActivity, await recentSearchesCache.GetMostPopular() },
                { IndexActivity, await recentIndexesCache.Get() }
            };
            return Ok(result);
        }

        [HttpPost("indexes/{word}")]
        [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.Created)]
        [Authorize(Policy = "AdminAndProLexicographers")]
        public async Task<IActionResult> PublishWord([FromRoute] string word)
        {
            var wordEntry = await wordEntryService.LoadEntry(word);
            if (wordEntry == null)
            {
                return BadRequest(ResponseHelper.GetResponseDict($"{word} not found in the repository so not indexed"));
            }

            var isWordAlreadyPublished = wordEntry.State.Equals(State.PUBLISHED);
            if (isWordAlreadyPublished)
            {
                return BadRequest(ResponseHelper.GetResponseDict($"{word} is already indexed"));
            }

            await wordEntryService.PublishEntry(wordEntry, User!.Identity!.Name!);
            return StatusCode((int)HttpStatusCode.Created, ResponseHelper.GetResponseDict($"{word} has been published"));
        }

        [HttpPost("indexes/batch")]
        [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.Created)]
        [Authorize(Policy = "AdminAndProLexicographers")]
        public async Task<IActionResult> PublishWords([FromBody] string[] words)
        {
            var entriesToIndex = new HashSet<WordEntry>();

            // TODO Later: Optimize by fetching all words in one database call instead of one-by-one.
            foreach (var word in words)
            {
                var entry = await wordEntryService.LoadEntry(word);
                if (entry != null && entry.State != State.PUBLISHED)
                {
                    entriesToIndex.Add(entry);
                }
            }

            if (entriesToIndex.Count == 0)
            {
                return NotFound(ResponseHelper.GetResponseDict("All words either do not exist or have already been indexed."));
            }

            foreach (var wordEntry in entriesToIndex)
            {
                // TODO Later: The words should be updated in one batch instead of one-by-one.
                await wordEntryService.PublishEntry(wordEntry, User!.Identity!.Name!);
            }

            var successMessage = $"The following words were successfully indexed: {string.Join(',', entriesToIndex.Select(x => x.Title))}";
            return StatusCode((int)HttpStatusCode.Created, ResponseHelper.GetResponseDict(successMessage));
        }

        [HttpDelete("indexes/{word}")]
        [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "AdminAndProLexicographers")]
        public async Task<IActionResult> UnpublishWord([FromRoute] string word)
        {
            var entry = await wordEntryService.FindByTitleAndState(word, State.PUBLISHED);

            if (entry == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ResponseHelper.GetResponseDict("Published word not found."));
            }

            entry.State = State.NEW;
            await wordEntryService.Update(entry);
            return Ok(ResponseHelper.GetResponseDict($"{word} removed from index."));
        }

        [HttpDelete("indexes/batch")]
        [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "AdminAndProLexicographers")]
        public async Task<IActionResult> UnpublishWords([FromBody] string[] words)
        {
            List<string> notFoundWords = new(), unpublishedWords = new();

            foreach (var word in words)
            {
                var entry = await wordEntryService.FindByTitleAndState(word, State.PUBLISHED);

                if (entry == null)
                {
                    notFoundWords.Add(word);
                }
                else
                {
                    entry.State = State.UNPUBLISHED;
                    await wordEntryService.Update(entry);
                    unpublishedWords.Add(entry.Title);
                }
            }

            if (!unpublishedWords.Any())
            {
                return NotFound(ResponseHelper.GetResponseDict("None of the words was found in the repository so not attempting to remove."));
            }

            var successMessage = $"Successfully removed the following words from search index: {string.Join(',', unpublishedWords)}.";

            if (notFoundWords.Any())
            {
                successMessage += $" The following words were skipped as they are not published in the database: {string.Join(',', notFoundWords)}.";
            }

            return Ok(ResponseHelper.GetResponseDict(successMessage));
        }
    }
}