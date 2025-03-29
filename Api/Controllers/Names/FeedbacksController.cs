using Application.Services;
using Core.Dto.Request;
using Core.Dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Api.Controllers.Names
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminAndLexicographers")]
    public class FeedbacksController : ControllerBase
    {
        private readonly NameEntryFeedbackService _nameEntryFeedbackService;

        private readonly NameEntryService _nameEntryService;

        public FeedbacksController(NameEntryFeedbackService nameEntryFeedbackService, NameEntryService nameEntryService)
        {
            _nameEntryFeedbackService = nameEntryFeedbackService;
            _nameEntryService = nameEntryService;
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(NameFeedbackDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetById(string id)
        {
            var feedback = await _nameEntryFeedbackService.GetFeedbackByIdAsync(id);

            if (feedback == null)
            {
                return NotFound();
            }

            return Ok(feedback);
        }

        /// <summary>
        /// Endpoint for getting all feedback or just name-specific feedback
        /// </summary>
        /// <param name="name">Optional name parameter</param>
        /// <returns>A list of all feedback (or just feedback for a given name)</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<NameFeedbackDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetFeedback([FromQuery] string? name = null)
        {
            var feedbacks = string.IsNullOrWhiteSpace(name) ? await _nameEntryFeedbackService.FindAllAsync() :
                await _nameEntryFeedbackService.FindByNameAsync(name);
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
        public async Task<IActionResult> Create([FromBody] CreateNameFeedbackDto model)
        {
            var nameEntry = await _nameEntryService.LoadEntry(model.Name);

            if (nameEntry == null)
            {
                return NotFound($"{model.Name} does not exist. Cannot add feedback");
            }

            await _nameEntryFeedbackService.AddFeedbackByNameAsync(model.Name, model.Feedback);

            return StatusCode((int)HttpStatusCode.Created, "Feedback added successfully.");
        }

        /// <summary>
        /// Endpoint for deleting a feedback for a name
        /// </summary>
        /// <param name="name"></param>
        /// <returns> A string containing outcome of action</returns>
        [HttpDelete]
        [Route("{name}/{feedbackId}")]
        [Authorize(Policy = "AdminAndProLexicographers")]
        public async Task<IActionResult> Delete(string name, string feedbackId)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name parameter is required.");
            }

            var nameEntry = await _nameEntryService.LoadEntry(name);

            if (nameEntry == null)
            {
                return NotFound("No feedback found with supplied name. None deleted");
            }

            var success = await _nameEntryFeedbackService.DeleteFeedbackAsync(name, feedbackId);

            if (success == false)
            {
                return NotFound("No feedback found with supplied id. None deleted");
            }

            return Ok(new { Message = $"Feedback message deleted" });
        }

        /// <summary>
        /// Endpoint for deleting all feedback for a name
        /// </summary>
        /// <param name="name"></param>
        /// <returns> A string containing outcome of action</returns>
        [HttpDelete]
        [Authorize(Policy = "AdminAndProLexicographers")]
        public async Task<IActionResult> DeleteAll([FromQuery][Required] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name parameter is required.");
            }

            var nameEntry = await _nameEntryService.LoadEntry(name);

            if (nameEntry == null)
            {
                string errorMsg = $"{name} does not exist. Cannot delete all feedback";
                return NotFound(errorMsg);
            }

            await _nameEntryFeedbackService.DeleteAllFeedbackForNameAsync(name);
            return Ok(new { Message = $"All Feedback messages deleted for {name}" });
        }
    }
}
