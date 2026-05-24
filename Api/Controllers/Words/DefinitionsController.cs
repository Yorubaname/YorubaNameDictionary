using Application.Services.Words;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Words.Core.Dto.Response;

namespace Api.Controllers.Words
{
    [Route("api/v1/words/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminAndLexicographers")]
    public class DefinitionsController(WordEntryService entryService) : ControllerBase
    {
        private const int DefaultPage = 1;
        private const int DefaultCount = 50;
        private const int MaxCount = 500;

        /// <summary>
        /// Returns English definitions for each requested word, irrespective of review status.
        /// </summary>
        [HttpGet("in-english")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetEnglishDefinitions([FromQuery] string words)
        {
            if (string.IsNullOrWhiteSpace(words))
            {
                return BadRequest("At least one word is required in query parameter words.");
            }

            var requestedWords = words
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .ToArray();

            var result = await entryService.GetEnglishDefinitionsOf(requestedWords);
            return Ok(result);
        }

        /// <summary>
        /// Adds English definitions that do not already exist for each supplied word.
        /// New definitions created via this endpoint are marked as needing review.
        /// </summary>
        [HttpPost("in-english")]
        [ProducesResponseType(typeof(IDictionary<string, object>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddEnglishDefinitions([FromBody] IDictionary<string, string>? payload)
        {
            if (payload == null || payload.Count == 0)
            {
                return BadRequest("Payload must be a dictionary of word to English definition.");
            }

            var currentUser = User?.Identity?.Name;
            var statuses = await entryService.AddEnglishDefinitionsAsync(payload, currentUser);
            var response = statuses.ToDictionary(kvp => kvp.Key, kvp => (object)new { status = kvp.Value });

            return Ok(response);
        }

        /// <summary>
        /// Lists words that contain at least one definition needing review.
        /// </summary>
        [HttpGet("needs-review")]
        [ProducesResponseType(typeof(WordDefinitionsNeedingReviewPageDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetWordsWithDefinitionsNeedingReview([FromQuery] int? page, [FromQuery] int? count)
        {
            var pageNumber = page ?? DefaultPage;
            var pageSize = Math.Min(count ?? DefaultCount, MaxCount);

            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("page and count must be greater than 0.");
            }

            var result = await entryService.GetWordsWithDefinitionsNeedingReviewAsync(pageNumber, pageSize);
            return Ok(result);
        }
    }
}
