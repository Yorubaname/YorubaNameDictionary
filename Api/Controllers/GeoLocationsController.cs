using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GeoLocationsController : ControllerBase
    {
        [Route("/geolocations")]
        [HttpGet]
        public IActionResult ListGeoLocations()
        {
            return Ok();
        }
    }
}
