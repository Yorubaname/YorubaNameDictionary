using Api.Mappers;
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

        public SearchController(
            SearchService searchService,
            IEventPubService eventPubService)
        {
            _searchService = searchService;
            _eventPubService = eventPubService;
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
    }
}
