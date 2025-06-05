using Api.Utilities;
using Application.Mappers.Words;
using Application.Services.Words;
using Core.Dto.Request;
using Core.Dto.Response;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Words.Core.Dto.Request;
using Words.Core.Dto.Response;
using Words.Core.Entities;
using YorubaOrganization.Core.Enums;

namespace Api.Controllers.Words
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminAndLexicographers")]
    public class WordsController(WordEntryService entryService, IValidator<CreateWordDto> createWordValidator) : ControllerBase
    {
        private const int DefaultPage = 1;
        private const int DefaultListCount = 50;
        private const int MaxListCount = 100; //TODO Later: Make configurable

        /// <summary>
        /// Create a new Word entry. Updates are not possible through this endpoint.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateWordDto request)
        {
            var result = await createWordValidator.ValidateAsync(request);
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
                return StatusCode((int)HttpStatusCode.Created, "Word successfully added");
            }
        }

        /// <summary>
        /// Fetch metadata about all words in the system.
        /// </summary>
        [HttpGet("meta")]
        [ProducesResponseType(typeof(WordsMetadataDto), (int)HttpStatusCode.OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetMetaData()
        {
            var metaData = await entryService.GetMetadata();
            return Ok(new WordsMetadataDto(
                metaData.TotalEntries,
                metaData.TotalNewEntries,
                metaData.TotalModifiedEntries,
                metaData.TotalPublishedEntries
            ));
        }

        /// <summary>
        /// List all words based on different filtering parameters.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(WordEntryDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(WordEntryMiniDto[]), (int)HttpStatusCode.OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllWords(
            [FromQuery] int? page,
            [FromQuery] int? count,
            [FromQuery] bool? all,
            [FromQuery] string? submittedBy,
            [FromQuery] State? state)
        {
            IEnumerable<WordEntry> words;
            if (all.HasValue && all.Value)
            {
                words = await entryService.GetAllEntries(state, submittedBy);
                return Ok(words.MapToDtoCollectionMini());
            }

            page ??= DefaultPage;
            count = Math.Min(count ?? DefaultListCount, MaxListCount);
            words = await entryService.List(state, submittedBy, page.Value, count.Value);
            return Ok(words.MapToDtoCollection());
        }

        /// <summary>
        /// Fetch a single word.
        /// </summary>
        [HttpGet("{word}")]
        [ProducesResponseType(typeof(WordEntryDto), (int)HttpStatusCode.OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetWord([FromRoute] string word)
        {
            var wordEntry = await entryService.LoadEntry(word);

            if (wordEntry == null)
            {
                string errorMsg = $"{word} not found in the database";
                return NotFound(errorMsg);
            }

            var wordToReturn = wordEntry.Modified ?? wordEntry;
            wordToReturn.State = wordEntry.State;
            wordToReturn.CreatedAt = wordEntry.CreatedAt;
            wordToReturn.UpdatedAt = wordEntry.UpdatedAt;

            return Ok(wordToReturn.MapToDto());
        }

        /// <summary>
        /// Updates a word entry.
        /// </summary>
        [HttpPut("{word}")]
        public async Task<IActionResult> UpdateWord(string word, [FromBody] UpdateWordDto updated)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var oldWordEntry = await entryService.LoadEntry(word);

            if (oldWordEntry == null)
            {
                return NotFound($"{word} not in database");
            }

            _ = await entryService.Update(oldWordEntry, updated.MapToEntity());
            return Ok(new { Message = "Word successfully updated" });
        }

        [HttpDelete("{word}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteWord(string word)
        {
            var wordEntry = await entryService.LoadEntry(word);
            if (wordEntry == null)
            {
                return NotFound($"{word} not found in the system so cannot be deleted");
            }

            await entryService.Delete(word);
            return Ok(new { Message = $"{word} Deleted" });
        }

        [HttpDelete("batch")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteWordsBatch(string[] words)
        {
            var foundWords = (await entryService.LoadEntries(words))?.Select(f => f.Title)?.ToArray();

            if (foundWords is null || foundWords.Length == 0)
            {
                return BadRequest("No deletion as none of the words were found in the database");
            }

            var notFoundWords = words.Where(d => !foundWords.Contains(d)).ToList();

            await entryService.DeleteEntriesBatch(foundWords);

            string responseMessage = string.Join(", ", foundWords) + " deleted";
            if (notFoundWords.Count > 0)
            {
                responseMessage += Environment.NewLine + string.Join(", ", notFoundWords) + " not deleted as they were not found in the database";
            }

            return Ok(new { Message = responseMessage });
        }
    }
}
