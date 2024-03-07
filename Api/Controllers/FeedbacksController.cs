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
        
        public FeedbacksController(NameEntryFeedbackService nameEntryFeedbackService)
        {
            _nameEntryFeedbackService = nameEntryFeedbackService;
        }


        [HttpGet]
        [ProducesResponseType(typeof(List<Feedback>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetFeedbacks()
        {
            var feedbacks = await _nameEntryFeedbackService.FindAll("CreatedAt");

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

            var feedbacks = await _nameEntryFeedbackService.FindByName(name, "CreatedAt");
            return Ok(feedbacks);
        }
    }
}
