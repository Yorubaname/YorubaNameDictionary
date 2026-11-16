using Api.Configuration;
using Api.Utilities;
using Application.Exceptions;
using Application.Mappers.Words;
using Application.Services.Words;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using Words.Core.Dto.Request;
using Words.Core.Dto.Response;

namespace Api.Controllers.Words
{
    [Route("api/v1/words/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminAndLexicographers")]
    public class SuggestionsController : ControllerBase
    {
        private const int DefaultPage = 1;
        private const int DefaultListCount = 50;

        private readonly SuggestionsService _suggestedWordService;
        private readonly IValidator<WordDto> _wordValidator;
        private readonly int _maxListCount;

        public SuggestionsController(SuggestionsService suggestedWordService, IValidator<WordDto> wordValidator, IOptions<WordsConfig> wordsConfig)
        {
            _suggestedWordService = suggestedWordService;
            _wordValidator = wordValidator;
            _maxListCount = wordsConfig.Value.MaxListCount;
        }

        /// <summary>
        /// List one page of suggested words, newest first. Always paged: defaults to page 1 of 50,
        /// and count is capped at the configured Words:MaxListCount.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(WordEntryDto[]), 200)]
        public async Task<IActionResult> List([FromQuery] int? page, [FromQuery] int? count)
        {
            var pageNumber = Math.Max(1, page ?? DefaultPage);
            var pageSize = Math.Clamp(count ?? DefaultListCount, 1, _maxListCount);
            var data = await _suggestedWordService.ListAsync(pageNumber, pageSize);
            return Ok(data.MapToDtoCollection());
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] CreateWordDto request)
        {
            var result = await _wordValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                await _suggestedWordService.CreateAsync(request.MapToEntity());
            }
            catch (WordAlreadyExistsException ex)
            {
                return Conflict(ResponseHelper.GetResponseDict(ex.Message));
            }

            return StatusCode((int)HttpStatusCode.Created, ResponseHelper.GetResponseDict("Suggested word successfully added"));
        }

        [HttpGet]
        [Route("meta")]
        [ProducesResponseType(typeof(Dictionary<string, int>), 200)]
        public async Task<IActionResult> GetMetaData()
        {
            var meta = await _suggestedWordService.CountAsync();
            return Ok(meta);
        }

        [HttpDelete]
        [Authorize(Policy = "AdminAndProLexicographers")]
        [Route("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _suggestedWordService.DeleteSuggestedNameAsync(id);

            if (result)
            {
                return NoContent();
            }

            return BadRequest($"Suggested word with id: {id} not found as a suggested word");
        }

        [HttpPost]
        [Authorize(Policy = "AdminAndProLexicographers")]
        [Route("{id}/accept")]
        [ProducesResponseType(typeof(WordEntryDto), 200)]
        public async Task<IActionResult> AcceptSuggestion(string id)
        {
            var acceptedWord = await _suggestedWordService.AcceptSuggestionAsync(id, User?.Identity?.Name);

            if (acceptedWord == null)
            {
                return BadRequest($"Suggested word with id: {id} not found as a suggested word");
            }

            return Ok(acceptedWord.MapToDto());
        }

        [HttpDelete]
        [Authorize(Policy = "AdminAndProLexicographers")]
        [Route("batch")]
        public async Task<IActionResult> DeleteBatch([FromBody] string[] words)
        {
            if (words is null || words.Length == 0)
            {
                return BadRequest("No deletion as no words were provided");
            }

            var deleteResult = await _suggestedWordService.DeleteSuggestedWordsBatchAsync(words);

            if (deleteResult.DeletedItems.Length == 0)
            {
                return BadRequest("No deletion as none of the words were found as suggestions in the database");
            }

            string responseMessage = string.Join(", ", deleteResult.DeletedItems) + " deleted";
            if (deleteResult.NotFoundItems.Length > 0)
            {
                responseMessage += Environment.NewLine + string.Join(", ", deleteResult.NotFoundItems) + " not deleted as they were not found in suggested words";
            }

            return Ok(new { Message = responseMessage });
        }
    }
}
