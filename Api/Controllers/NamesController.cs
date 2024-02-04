using Api.Mappers;
using Api.Model.In;
using Api.Model.Out;
using Application.Domain;
using Core.Dto;
using Core.Entities.NameEntry;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class NamesController :  ControllerBase
    {
        private readonly NameEntryService _nameEntryService;
        public NamesController(NameEntryService entryService)
        {
            _nameEntryService = entryService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateNameDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                if (request.State.HasValue && request.State != State.NEW)
                {
                    return BadRequest("Invalid State: A new entry needs to have the NEW state");
                }

                await _nameEntryService.Create(request.MapToEntity());
                return StatusCode((int)HttpStatusCode.Created, "Name successfully added");
            }
        }

        [HttpGet("meta")]
        [ProducesResponseType(typeof(NamesMetadataDto[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMetaData()
        {
            var metaData = await _nameEntryService.GetMetadata();

            return Ok(metaData);
        }

        [HttpGet]
        [ProducesResponseType(typeof(NameEntryDto[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllNames(
        [FromQuery] int? page,
        [FromQuery] int? count,
        [FromQuery] bool? all,
        [FromQuery] string? submittedBy,
        [FromQuery] State? state)
        {
            List<NameEntry>? names = null;

            // TODO: Move logic into application layer
            if (all.HasValue && all.Value)
            {
                if (state.HasValue)
                {
                    names = await _nameEntryService.FindBy(state.Value);
                }
                else
                {
                    names = await _nameEntryService.ListNames();
                }
            }
            else if(state != null)
            {
                names = await _nameEntryService.FindBy(state.Value, page, count);
            }

            // TODO: Do this filtering at database level to reduce waste of I/O
            names = names == null ?  new List<NameEntry>() : names
                .Where(n => string.IsNullOrWhiteSpace(submittedBy) || n.CreatedBy.Equals(submittedBy.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(names.MapToDtoCollection());
        }
    }
}
