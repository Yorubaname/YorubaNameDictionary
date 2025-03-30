using Application.Services.Words;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Words.Core.Dto.Request;
using Words.Core.Dto.Response;

namespace Api.Controllers.Words
{
    [Route("api/v1/dictionary/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminAndLexicographers")]
    public class FeedbackController : ControllerBase
    {
        private readonly WordFeedbackService _wordEntryFeedbackService;
        private readonly WordEntryService _wordEntryService;

        public FeedbackController(WordFeedbackService wordEntryFeedbackService, WordEntryService wordEntryService)
        {
            _wordEntryFeedbackService = wordEntryFeedbackService;
            _wordEntryService = wordEntryService;
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(WordFeedbackDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetById(string id)
        {
            var feedback = await _wordEntryFeedbackService.GetFeedbackByIdAsync(id);

            if (feedback == null)
            {
                return NotFound();
            }

            return Ok(feedback);
        }

        /// <summary>
        /// Endpoint for getting all feedback or just word-specific feedback
        /// </summary>
        /// <param name="word">Optional word parameter</param>
        /// <returns>A list of all feedback (or just feedback for a given word)</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<WordFeedbackDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetFeedback([FromQuery] string? word = null)
        {
            var feedbacks = string.IsNullOrWhiteSpace(word)
                ? await _wordEntryFeedbackService.FindAllAsync()
                : await _wordEntryFeedbackService.FindByWordAsync(word);

            return Ok(feedbacks);
        }

        /// <summary>
        /// Endpoint for adding a feedback
        /// </summary>
        /// <param name="model"></param>
        /// <returns>A string indicating the status of the operation</returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] CreateFeedbackDto model)
        {
            var wordEntry = await _wordEntryService.LoadEntry(model.Word);

            if (wordEntry == null)
            {
                return NotFound($"{model.Word} does not exist. Cannot add feedback");
            }

            await _wordEntryFeedbackService.AddFeedbackForWordAsync(model.Word, model.Feedback);

            return StatusCode((int)HttpStatusCode.Created, "Feedback added successfully.");
        }

        /// <summary>
        /// Endpoint for deleting a feedback for a word
        /// </summary>
        /// <param name="word"></param>
        /// <param name="feedbackId"></param>
        /// <returns>A string containing outcome of action</returns>
        [HttpDelete]
        [Route("{word}/{feedbackId}")]
        [Authorize(Policy = "AdminAndProLexicographers")]
        public async Task<IActionResult> Delete(string word, string feedbackId)
        {
            if (string.IsNullOrEmpty(word))
            {
                return BadRequest("Word parameter is required.");
            }

            var wordEntry = await _wordEntryService.LoadEntry(word);

            if (wordEntry == null)
            {
                return NotFound("No feedback found with the supplied word. None deleted");
            }

            var success = await _wordEntryFeedbackService.DeleteFeedbackAsync(word, feedbackId);

            if (!success)
            {
                return NotFound("No feedback found with the supplied id. None deleted");
            }

            return Ok(new { Message = "Feedback message deleted" });
        }

        /// <summary>
        /// Endpoint for deleting all feedback for a word
        /// </summary>
        /// <param name="word"></param>
        /// <returns>A string containing outcome of action</returns>
        [HttpDelete]
        [Authorize(Policy = "AdminAndProLexicographers")]
        public async Task<IActionResult> DeleteAll([FromQuery][Required] string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return BadRequest("Word parameter is required.");
            }

            var wordEntry = await _wordEntryService.LoadEntry(word);

            if (wordEntry == null)
            {
                string errorMsg = $"{word} does not exist. Cannot delete all feedback";
                return NotFound(errorMsg);
            }

            await _wordEntryFeedbackService.DeleteAllFeedbackForWordAsync(word);
            return Ok(new { Message = $"All feedback messages deleted for {word}" });
        }
    }
}