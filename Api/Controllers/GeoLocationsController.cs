using Application.Domain;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GeoLocationsController : ControllerBase
    {
        private readonly GeoLocationsService _geoLocationsService;
        public GeoLocationsController(GeoLocationsService geoLocationsService)
        {
            _geoLocationsService = geoLocationsService;
        }

        [HttpGet]
        public async Task<IActionResult> ListGeoLocations()
        {
            var result = await _geoLocationsService.GetAll();
            return Ok(result);
        }
    }
}
