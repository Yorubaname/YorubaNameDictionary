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


        [HttpGet]
        [ProducesResponseType(typeof(List<Feedback>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetFeedbacks()
        {
            var feedbacks = await _nameEntryFeedbackService.FindAllAsync("CreatedAt");

            return Ok(feedbacks);
        }

        [HttpGet]
        [Route("{name}")]
        [ProducesResponseType(typeof(List<Feedback>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetFeedbacksForName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name parameter is required.");
            }

            var feedbacks = await _nameEntryFeedbackService.FindByNameAsync(name, "CreatedAt");
            return Ok(feedbacks);
        }
        
        [HttpPost]
        [ProducesResponseType(typeof(List<Feedback>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddFeedback([FromBody] CreateNameFeedbackDto model)
        {
            var nameEntry = await _nameEntryService.LoadName(model.Name);

            if (nameEntry == null)
            {
                string errorMsg = $"{model.Name} does not exist. Cannot add feedback";
                return NotFound(errorMsg);
            }

            var result = await _nameEntryFeedbackService
                .AddFeedbackByNameAsync(model.Name, model.FeedbackContent);

            return Ok(result ? "Feedback added": "Something went wrong!...");
        }
    }
}
