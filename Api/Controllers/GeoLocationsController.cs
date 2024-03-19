using Application.Services;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        /// <summary>
        /// End point for returning the locations a name entry could be from
        /// </summary>        
        /// <returns>
        /// An <see cref="GeoLocation[]"/> representing the response containing the list of <see cref="GeoLocation"/> objects.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(GeoLocation[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListGeoLocations()
        {
            var result = await _geoLocationsService.GetAll();
            return Ok(result);
        }
    }
}
