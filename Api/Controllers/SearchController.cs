using Application.Services;
using Core.Dto.Response;
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

        public SearchController(SearchService searchService) 
        {
            _searchService = searchService;
        }

        [HttpGet("meta")]
        [ProducesResponseType(typeof(SearchMetadataDto[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMetadata()
        {
            var metadata = await _searchService.GetNamesMetadata();
            return Ok(metadata);
        }

        [HttpGet]
        [ProducesResponseType(typeof(SearchMetadataDto[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMetadata([FromQuery(Name ="q"), Required] string searchTerm)
        {
            IEnumerable<NameEntryDto> matches = await _searchService.Search(searchTerm);
            // TODO Hafiz: Publish event if exact match
            return Ok(matches);
        }
    }
}
