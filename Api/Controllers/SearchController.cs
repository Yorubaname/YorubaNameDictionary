using Api.Mappers;
using Application.Cache;
using Application.Services;
using Core.Dto.Response;
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
        private readonly IEventPubService _eventPubService;
        private readonly RecentSearchesCache _recentSearchesCache;
        private readonly RecentIndexesCache _recentIndexesCache;

        public SearchController(
            SearchService searchService,
            IEventPubService eventPubService,
            RecentSearchesCache recentSearchesCache,
            RecentIndexesCache recentIndexesCache)
        {
            _searchService = searchService;
            _eventPubService = eventPubService;
            _recentSearchesCache = recentSearchesCache;
            _recentIndexesCache = recentIndexesCache;
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

        [HttpGet("activity/all")]
        public async Task<IActionResult> GetRecent()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityType">Possible values: "search", "index", "popular"</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("activity")]
        public async Task<IActionResult> GetRecentByActivity([FromQuery(Name = "q")] string? activityType = null)
        {
            if (string.IsNullOrEmpty(activityType))
            {
                return RedirectToAction(nameof(GetAllActivity));
            }

            if (activityType.Equals("search", StringComparison.OrdinalIgnoreCase))
            {
                return Ok(await _recentSearchesCache.Get());
            }

            if (activityType.Equals("index", StringComparison.OrdinalIgnoreCase))
            {
                return Ok(await _recentIndexesCache.Get());
            }

            if (activityType.Equals("popular", StringComparison.OrdinalIgnoreCase))
            {
                return  Ok(await _recentSearchesCache.GetMostPopular());
            }

            throw new Exception("Activity type not recognized");
        }
    }
}
