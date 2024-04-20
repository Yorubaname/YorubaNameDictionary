using Api.Mappers;
using Api.Model.In;
using Application.Domain;
using Core.Dto.Request;
using Core.Dto.Response;
using Core.Entities.NameEntry;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class NamesController : ControllerBase
    {
        private readonly NameEntryService _nameEntryService;

        public NamesController(NameEntryService entryService)
        {
            _nameEntryService = entryService;
        }

        /// <summary>
        /// CreateAsync a new Name entries. Updates are not possible through this endpoint.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Fetch metadata about all names in the system.
        /// </summary>
        /// <returns></returns>
        [HttpGet("meta")]
        [ProducesResponseType(typeof(NamesMetadataDto[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMetaData()
        {
            var metaData = await _nameEntryService.GetMetadata();

            return Ok(metaData);
        }

        /// <summary>
        /// List all names based on different filtering parameters.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <param name="all"></param>
        /// <param name="submittedBy"></param>
        /// <param name="state"></param>
        /// <returns></returns>
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
            else if (state != null)
            {
                names = await _nameEntryService.FindBy(state.Value, page, count);
            }

            // TODO Minor: Do this filtering at database level to reduce waste of I/O
            names = names == null ? new List<NameEntry>() : names
                .Where(n => string.IsNullOrWhiteSpace(submittedBy) || n.CreatedBy.Equals(submittedBy.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(names.MapToDtoCollection());
        }

        /// <summary>
        /// Fetch a single name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name}")]
        [ProducesResponseType(typeof(NameEntryDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetName([FromRoute] string name)
        {
            var nameEntry = await _nameEntryService.LoadName(name);

            if (nameEntry == null)
            {
                string errorMsg = $"{name} not found in the database";
                return NotFound(errorMsg);
            }

            return Ok(nameEntry.MapToDto());
        }

        /// <summary>
        /// Updates a name entry.
        /// </summary>
        /// <param name="name">The name path variable</param>
        /// <param name="updated">The NameEntry object</param>
        /// <returns></returns>
        [HttpPut("{name}")]
        public async Task<IActionResult> UpdateName(string name, [FromBody] UpdateNameDto updated)
        {
            // TODO: Ensure that before connecting the dashboard to this endpoint, it follows the new update paradigm of only one active edit at a time
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var oldNameEntry = await _nameEntryService.LoadName(name);

            if (oldNameEntry == null)
            {
                return NotFound($"{name} not in database");
            }

            // TODO: Ensure possible errors from calling UpdateName are handled in the global exception handling middleware
            _ = await _nameEntryService.UpdateName(oldNameEntry, updated.MapToEntity());
            return Ok(new { Message = "Name successfully updated" });
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteName(string name)
        {
            var nameEntry = await _nameEntryService.LoadName(name);
            if (nameEntry == null)
            {
                return NotFound($"{name} not found in the system so cannot be deleted");
            }

            await _nameEntryService.Delete(name);
            return Ok(new { Message = $"{name} Deleted" });
        }
    }
}
