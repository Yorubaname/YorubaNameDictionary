using Api.Mappers;
using Application.Cache;
using Application.Domain;
using Application.Events;
using Application.Services;
using Core.Cache;
using Core.Dto.Response;
using Core.Entities.NameEntry;
using Core.Enums;
using Core.Events;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

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

            if (matches.Count() == 1 && matches.First().Name.ToLower() == searchTerm.ToLower())
            {
                await _eventPubService.PublishEvent(new ExactNameSearched(searchTerm));
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

            return Ok(await _searchService.SearchByStartsWith(searchTerm));
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
            // TODO Hafiz: Test that the action is executed when there is no "q" parameter
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
        public async Task<IActionResult> PublishName([FromRoute] string name)
        {
            var response = new Dictionary<string, string>();
            var nameEntry = await _nameEntryService.LoadName(name);
            if (nameEntry == null)
            {
                response.Add("message", $"{name} not found in the repository so not indexed");
                return BadRequest(response);
            }

            var isNameAlreadyPublished = nameEntry.State.Equals(State.PUBLISHED);
            if (isNameAlreadyPublished)
            {
                response.Add("message", $"{name} is already indexed");
                return BadRequest(response);
            }

            await PublishName(nameEntry);

            response.Add("message", $"{name} has been published");
            return StatusCode((int)HttpStatusCode.Created, response);
        }

        /// <summary>
        /// Publish a collection of existing names to the index.
        /// </summary>
        /// <returns></returns>
        [HttpPost("indexes/batch")]
        [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> PublishNames([FromBody] string[] names)
        {
            var response = new Dictionary<string, object>();
            var entriesToIndex = new HashSet<NameEntry>();

            foreach (var name in names)
            {
                // TODO Hafiz (Future improvements): Would be better as a bulk load.
                var entry = await _nameEntryService.LoadName(name);
                if (entry != null && entry.State != State.PUBLISHED)
                {
                    entriesToIndex.Add(entry);
                }
            }

            if (entriesToIndex.Count == 0)
            {
                response.Add("message", "all names either do not exist or have already been indexed.");
                return NotFound(response);
            }

            foreach (var nameEntry in entriesToIndex)
            {
                // TODO Hafiz: This should be transactional or in a batch but this would do for now since frequent use is not anticipated.
                await PublishName(nameEntry);
            }

            response.Add("message", $"The following names were successfully indexed: {string.Join(',', entriesToIndex.Select(x => x.Name))}");
            return StatusCode((int)HttpStatusCode.Created, response);
        }

        private async Task PublishName(NameEntry nameEntry)
        {
            nameEntry.State = State.PUBLISHED;
            await _nameEntryService.UpdateName(nameEntry);
            // TODO Hafiz: Ideally, this would be in a transaction with the update, but for now, not important
            await _eventPubService.PublishEvent(new NameIndexed(nameEntry.Name));
        }

        /// <summary>
        /// Remove a name from the index.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("indexes/{name}")]
        public async Task<IActionResult> UnpublishName([FromRoute] string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove a name from the index.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("indexes/batch")]
        public async Task<IActionResult> UnpublishNames([FromBody] string[] names)
        {
            throw new NotImplementedException();
        }
    }
}
