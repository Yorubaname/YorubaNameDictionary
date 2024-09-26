using Api.Utilities;
using Application.Domain;
using Application.Mappers;
using Application.Services;
using Core.Cache;
using Core.Dto.Response;
using Core.Entities;
using Core.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using YorubaOrganization.Core.Enums;

namespace Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly SearchService _searchService;
        private readonly NameEntryService _nameEntryService;
        private readonly IEventPubService _eventPubService;
        private readonly IRecentSearchesCache _recentSearchesCache;
        private readonly IRecentIndexesCache _recentIndexesCache;

        private const string SearchActivity = "search", IndexActivity = "index", PopularActivity = "popular";

        public SearchController(
            SearchService searchService,
            IEventPubService eventPubService,
            IRecentSearchesCache recentSearchesCache,
            IRecentIndexesCache recentIndexesCache,
            NameEntryService nameEntryService)
        {
            _searchService = searchService;
            _eventPubService = eventPubService;
            _recentSearchesCache = recentSearchesCache;
            _recentIndexesCache = recentIndexesCache;
            _nameEntryService = nameEntryService;
        }

        [HttpGet("meta")]
        [ProducesResponseType(typeof(SearchMetadataDto[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMetadata()
        {
            var metadata = await _searchService.GetNamesMetadata();
            return Ok(metadata);
        }

        [HttpGet]
        [ProducesResponseType(typeof(NameEntryDto[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Search([FromQuery(Name = "q"), Required] string searchTerm)
        {
            var matches = await _searchService.Search(searchTerm);

            // TODO: Check if the comparison here removes takes diacrits into consideration
            if (matches.Count() == 1 && matches.First().Title.Equals(searchTerm, StringComparison.CurrentCultureIgnoreCase))
            {
                await _eventPubService.PublishEvent(new ExactNameSearched(matches.First().Title));
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

            return Ok(await _searchService.AutoComplete(searchTerm));
        }

        [HttpGet("alphabet/{searchTerm}")]
        [ProducesResponseType(typeof(IEnumerable<NameEntryDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchByStartsWith(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Ok(Enumerable.Empty<NameEntryDto>());
            }

            var nameEntry = await _searchService.SearchByStartsWith(searchTerm);
            return Ok(nameEntry.MapToDtoCollection());
        }

        [HttpGet("{searchTerm}")]
        [ProducesResponseType(typeof(NameEntryDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchOne(string searchTerm)
        {
            var nameEntry = await _searchService.GetName(searchTerm);

            if(nameEntry != null)
            {
                await _eventPubService.PublishEvent(new ExactNameSearched(nameEntry.Name));
            }

            return Ok(nameEntry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityType">Possible values: "search", "index", "popular"</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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
                return Ok(await _recentSearchesCache.Get());
            }

            if (activityType.Equals(IndexActivity, StringComparison.OrdinalIgnoreCase))
            {
                return Ok(await _recentIndexesCache.Get());
            }

            if (activityType.Equals(PopularActivity, StringComparison.OrdinalIgnoreCase))
            {
                return Ok(await _recentSearchesCache.GetMostPopular());
            }

            return BadRequest("Activity type not recognized");
        }

        [HttpGet("activity/all")]
        [ProducesResponseType(typeof(Dictionary<string, IEnumerable<string>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRecentStats()
        {
            var result = new Dictionary<string, IEnumerable<string>>()
            {
                { SearchActivity, await _recentSearchesCache.Get() },
                { PopularActivity, await _recentSearchesCache.GetMostPopular() },
                { IndexActivity, await _recentIndexesCache.Get() }
            };
            return Ok(result);
        }

        /// <summary>
        /// Publish an existing name to the index.
        /// </summary>
        /// <returns></returns>
        [HttpPost("indexes/{name}")]
        [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.Created)]
        [Authorize(Policy = "AdminAndProLexicographers")]
        public async Task<IActionResult> PublishName([FromRoute] string name)
        {
            var nameEntry = await _nameEntryService.LoadName(name);
            if (nameEntry == null)
            {
                return BadRequest(ResponseHelper.GetResponseDict($"{name} not found in the repository so not indexed"));
            }

            var isNameAlreadyPublished = nameEntry.State.Equals(State.PUBLISHED);
            if (isNameAlreadyPublished)
            {
                return BadRequest(ResponseHelper.GetResponseDict($"{name} is already indexed"));
            }

            await _nameEntryService.PublishName(nameEntry, User!.Identity!.Name!);
            return StatusCode((int)HttpStatusCode.Created, ResponseHelper.GetResponseDict($"{name} has been published"));
        }

        /// <summary>
        /// Publish a collection of existing names to the index.
        /// </summary>
        /// <returns></returns>
        [HttpPost("indexes/batch")]
        [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.Created)]
        [Authorize(Policy = "AdminAndProLexicographers")]
        public async Task<IActionResult> PublishNames([FromBody] string[] names)
        {
            var entriesToIndex = new HashSet<NameEntry>();


            // TODO Later: Optimize by fetching all names in one database call instead of one-by-one.
            foreach (var name in names)
            {
                var entry = await _nameEntryService.LoadName(name);
                if (entry != null && entry.State != State.PUBLISHED)
                {
                    entriesToIndex.Add(entry);
                }
            }

            if (entriesToIndex.Count == 0)
            {
                return NotFound(ResponseHelper.GetResponseDict("All names either do not exist or have already been indexed."));
            }

            foreach (var nameEntry in entriesToIndex)
            {
                // TODO Later: The names should be updated in one batch instead of one-by-one.
                await _nameEntryService.PublishName(nameEntry, User!.Identity!.Name!);
            }

            var successMessage = $"The following names were successfully indexed: {string.Join(',', entriesToIndex.Select(x => x.Title))}";
            return StatusCode((int)HttpStatusCode.Created, ResponseHelper.GetResponseDict(successMessage));
        }

        /// <summary>
        /// Remove a name from the index.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("indexes/{name}")]
        [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "AdminAndProLexicographers")]
        public async Task<IActionResult> UnpublishName([FromRoute] string name)
        {
            var entry = await _nameEntryService.FindByNameAndState(name, State.PUBLISHED);

            if (entry == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ResponseHelper.GetResponseDict("Published name not found."));
            }

            entry.State = State.NEW;
            await _nameEntryService.UpdateName(entry);
            return Ok(ResponseHelper.GetResponseDict($"{name} removed from index."));
        }

        /// <summary>
        /// Remove a name from the index.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("indexes/batch")]
        [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "AdminAndProLexicographers")]
        public async Task<IActionResult> UnpublishNames([FromBody] string[] names)
        {
            List<string> notFoundNames = new(), unpublishedNames = new();

            foreach (var name in names)
            {
                var entry = await _nameEntryService.FindByNameAndState(name, State.PUBLISHED);

                if (entry == null)
                {
                    notFoundNames.Add(name);
                }
                else
                {
                    entry.State = State.UNPUBLISHED;
                    await _nameEntryService.UpdateName(entry);
                    unpublishedNames.Add(entry.Title);
                }
            }

            if (!unpublishedNames.Any())
            {
                return NotFound(ResponseHelper.GetResponseDict("None of the names was found in the repository so not attempting to remove."));
            }

            var successMessage = $"Successfully removed the following names from search index: {string.Join(',', unpublishedNames)}.";

            if (notFoundNames.Any())
            {
                successMessage += $" The following names were skipped as they are not published in the database: {string.Join(',', notFoundNames)}.";
            }

            return Ok(ResponseHelper.GetResponseDict(successMessage));
        }
    }
}
