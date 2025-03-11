using Api.Utilities;
using Application.Mappers;
using Application.Services;
using Core.Dto.Request;
using Core.Dto.Response;
using Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Net;
using YorubaOrganization.Core.Enums;

namespace Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminAndLexicographers")]
    public class NamesController(NameEntryService entryService, IValidator<NameDto> createNameValidator) : ControllerBase
    {
        private const int DefaultPage = 1;
        private const int DefaultListCount = 50;
        private const int MaxListCount = 100; //TODO Later: Make configurable

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
            var result = await createNameValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }
            else
            {
                if (request.State.HasValue && request.State != State.NEW)
                {
                    return BadRequest("Invalid State: A new entry needs to have the NEW state");
                }

                await entryService.Create(request.MapToEntity());
                return StatusCode((int)HttpStatusCode.Created, "Name successfully added");
            }
        }

        /// <summary>
        /// Fetch metadata about all names in the system.
        /// </summary>
        /// <returns></returns>
        [HttpGet("meta")]
        [ProducesResponseType(typeof(NamesMetadataDto), (int)HttpStatusCode.OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetMetaData()
        {
            var metaData = await entryService.GetMetadata();
            return Ok(new NamesMetadataDto
            {
                TotalNames = metaData.TotalEntries,
                TotalPublishedNames = metaData.TotalPublishedEntries,
                TotalModifiedNames = metaData.TotalModifiedEntries,
                TotalNewNames = metaData.TotalNewEntries
            });
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
        [ProducesResponseType(typeof(NameEntryMiniDto[]), (int)HttpStatusCode.OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllNames(
        [FromQuery] int? page,
        [FromQuery] int? count,
        [FromQuery] bool? all,
        [FromQuery] string? submittedBy,
        [FromQuery] State? state)
        {
            // TODO: Eventually create different endpoints for this to eliminate the IF statements.
            IEnumerable<NameEntry> names;
            if (all.HasValue && all.Value)
            {
                names = await entryService.GetAllEntries(state, submittedBy);
                return Ok(names.MapToDtoCollectionMini());
            }

            page ??= DefaultPage;
            count = Math.Min(count ?? DefaultListCount, MaxListCount);
            names = await entryService.List(state, submittedBy, page.Value, count.Value);
            return Ok(names.MapToDtoCollection());
        }

        /// <summary>
        /// Fetch a single name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name}")]
        [ProducesResponseType(typeof(NameEntryDto), (int)HttpStatusCode.OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetName([FromRoute] string name)
        {
            var nameEntry = await entryService.LoadEntry(name);

            if (nameEntry == null)
            {
                string errorMsg = $"{name} not found in the database";
                return NotFound(errorMsg);
            }

            var nameToReturn = nameEntry.Modified ?? nameEntry;
            nameToReturn.State = nameEntry.State;
            nameToReturn.CreatedAt = nameEntry.CreatedAt;
            nameToReturn.UpdatedAt = nameEntry.UpdatedAt;

            return Ok(nameToReturn.MapToDto());
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var oldNameEntry = await entryService.LoadEntry(name);

            if (oldNameEntry == null)
            {
                return NotFound($"{name} not in database");
            }

            _ = await entryService.Update(oldNameEntry, updated.MapToEntity());
            return Ok(new { Message = "Name successfully updated" });
        }

        [HttpDelete("{name}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteName(string name)
        {
            var nameEntry = await entryService.LoadEntry(name);
            if (nameEntry == null)
            {
                return NotFound($"{name} not found in the system so cannot be deleted");
            }

            await entryService.Delete(name);
            return Ok(new { Message = $"{name} Deleted" });
        }


        [HttpDelete("batch")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteNamesBatch(string[] names)
        {

            var foundNames = (await entryService.LoadEntries(names))?.Select(f => f.Title)?.ToArray();

            if (foundNames is null || foundNames.Length == 0)
            {
                return BadRequest("No deletion as none of the names were found in the database");
            }

            var notFoundNames = names.Where(d => !foundNames.Contains(d)).ToList();

            await entryService.DeleteEntriesBatch(foundNames);

            string responseMessage = string.Join(", ", foundNames) + " deleted";
            if (notFoundNames.Count > 0)
            {
                responseMessage += Environment.NewLine + string.Join(", ", notFoundNames) + " not deleted as they were not found in the database";
            }

            return Ok(new { Message = responseMessage });
        }
    }
}
