using Microsoft.AspNetCore.Mvc;
using YorubaOrganization.Core.Repositories;

namespace Api.Controllers.Names
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EtymologyController : ControllerBase
    {
        private readonly IEtymologyRepository _etymologyRepository;

        public EtymologyController(IEtymologyRepository etymologyRepository)
        {
            _etymologyRepository = etymologyRepository;
        }

        [HttpGet("latest-meaning")]
        public async Task<IActionResult> GetLatestMeaning([FromQuery] string parts)
        {
            if (string.IsNullOrWhiteSpace(parts))
            {
                return BadRequest("Parts parameter is required.");
            }

            var partsList = parts.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim());
            var result = await _etymologyRepository.GetLatestMeaningOf(partsList);
            return Ok(result);
        }
    }
}
