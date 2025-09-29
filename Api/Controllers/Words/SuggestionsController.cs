using Api.Utilities;
using Application.Mappers.Words;
using Application.Services.Words;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly SuggestionsService _suggestedWordService;
        private readonly IValidator<WordDto> _wordValidator;

        public SuggestionsController(SuggestionsService suggestedWordService, IValidator<WordDto> wordValidator)
        {
            _suggestedWordService = suggestedWordService;
            _wordValidator = wordValidator;
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

            await _suggestedWordService.CreateAsync(request.MapToEntity());

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

        [HttpGet]
        [ProducesResponseType(typeof(WordEntryDto[]), 200)]
        public async Task<IActionResult> GetAll()
        {
            var data = await _suggestedWordService.GetAllAsync();
            return Ok(data.MapToDtoCollection());
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
    }
}
