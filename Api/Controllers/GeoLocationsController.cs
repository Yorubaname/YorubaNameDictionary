using Api.Utilities;
using Application.Services;
using Core.Dto.Request;
using Core.Dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using YorubaOrganization.Core.Entities;

namespace Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminAndProLexicographers")]
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
        [AllowAnonymous]
        [ProducesResponseType(typeof(GeoLocationDto[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListGeoLocations()
        {
            var result = (await _geoLocationsService.GetAll()).Select(g => new GeoLocationDto(g.Id, g.Place, g.Region));
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(GeoLocationDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Create(CreateGeoLocationDto geo)
        {
            var geoLocation = new GeoLocation(geo.Place, geo.Region)
            {
                CreatedBy = User!.Identity!.Name!
            };
            await _geoLocationsService.Create(geoLocation);
            return StatusCode((int)HttpStatusCode.Created, ResponseHelper.GetResponseDict("Geolocation successfully added"));
        }

        [HttpDelete("{id}/{place}")]
        [ProducesResponseType(typeof(GeoLocationDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(string id, string place)
        {
            await _geoLocationsService.Delete(id, place);
            return Ok(ResponseHelper.GetResponseDict($"Geolocation '{place}' successfully deleted"));
        }
    }
}
