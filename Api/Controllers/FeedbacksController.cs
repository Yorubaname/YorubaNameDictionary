using Api.Model.Request;
using Application.Domain;
using Application.Services;
using Core.Entities.NameEntry.Collections;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly NameEntryFeedbackService _nameEntryFeedbackService;       

        private readonly NameEntryService _nameEntryService;

        public FeedbacksController(NameEntryFeedbackService nameEntryFeedbackService, NameEntryService nameEntryService)
        {
            _nameEntryFeedbackService = nameEntryFeedbackService;
            _nameEntryService = nameEntryService;
        }

        /// <summary>
        /// Endpoint for getting all feedback within the system
        /// </summary>
        /// <returns> A list of all feedback</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Feedback>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var feedbacks = await _nameEntryFeedbackService.FindAllAsync();

            return Ok(feedbacks);
        }

        /// <summary>
        /// Endpoint for adding a feedback
        /// </summary>
        /// <param name="model"></param>
        /// <returns>A string indicating the status of the operation</returns>
        [HttpPost]
        [ProducesResponseType(typeof(List<Feedback>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Create([FromBody] CreateNameFeedbackDto model)
        {
            var nameEntry = await _nameEntryService.LoadName(model.Name);

            if (nameEntry == null)
            {
                return NotFound($"{model.Name} does not exist. Cannot add feedback");
            }

            await _nameEntryFeedbackService
                .AddFeedbackByNameAsync(model.Name, model.FeedbackContent);
            
            return StatusCode((int)HttpStatusCode.Created, "Feedback added successfully.");
        }

        /// <summary>
        /// Endpoint for getting all feedback within the system for a given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A list of all feedback for a given name</returns>
        [HttpGet]
        [Route("{name}")]
        [ProducesResponseType(typeof(List<Feedback>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name parameter is required.");
            }

            var feedbacks = await _nameEntryFeedbackService.FindByNameAsync(name);
            return Ok(feedbacks);
        }

        /// <summary>
        /// Endpoint for deleting all feedback for a name
        /// </summary>
        /// <param name="name"></param>
        /// <returns> A string containing outcome of action</returns>
        [HttpDelete]
        [Route("{name}")]
        public async Task<IActionResult> DeleteAll(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name parameter is required.");
            }

            var nameEntry = await _nameEntryService.LoadName(name);

            if (nameEntry == null)
            {
                string errorMsg = $"{name} does not exist. Cannot delete all feedback";
                return NotFound(errorMsg);
            }

            var success = await _nameEntryFeedbackService.DeleteAllFeedbackForNameAsync(name);

            if (!success)
            {
                return StatusCode(500, "Something went wrong!...");
            }

            return Ok($"All Feedback messages deleted for {name}");
        }

        [HttpGet]
        [Route("id")]
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
        /// Endpoint for deleting a feedback for a name
        /// </summary>
        /// <param name="name"></param>
        /// <returns> A string containing outcome of action</returns>
        [HttpDelete]
        [Route("{name}/{feedbackId}")]
        public async Task<IActionResult> Delete(string name, string feedbackId)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name parameter is required.");
            }

            var nameEntry = await _nameEntryService.LoadName(name);

            if (nameEntry == null)
            {
                return NotFound("No feedback found with supplied name. None deleted");
            }

            var success = await _nameEntryFeedbackService.DeleteFeedbackAsync(name, feedbackId);

            if (!success)
            {
                return NotFound("No feedback found with supplied Id. None deleted.");
            }

            return Ok("Feedback message deleted");
        }
    }
}
