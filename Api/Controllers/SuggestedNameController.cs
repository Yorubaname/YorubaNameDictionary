using Api.Mappers;
using Application.Services;
using Core.Dto.Request;
using Core.Dto.Response;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers;

[Route("api/v1/suggestions")]
[ApiController]
public class SuggestedNameController : ControllerBase
{
    private readonly SuggestedNameService _suggestedNameService;

    public SuggestedNameController(SuggestedNameService suggestedNameService)
    {
        _suggestedNameService = suggestedNameService;
    }

    [HttpGet]
    [Route("meta")]
    [ProducesResponseType(typeof(Dictionary<string, int>), 200)]
    public async Task<IActionResult> GetMetaData()
    {
        var suggestname = await _suggestedNameService.CountAsync();

        return Ok(suggestname);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SuggestedNameDto), 200)]
    public async Task<IActionResult> Create([FromBody] CreateSuggestedNameDto request)
    {
        await _suggestedNameService
                .CreateAsync(request.MapToEntity());

        return StatusCode((int)HttpStatusCode.Created, "Suggested Name successfully added");
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(SuggestedNameDto[]), 200)]
    public async Task<IActionResult> GetAll()
    {
        var data = await _suggestedNameService.GetAllAsync();
        return Ok(data.MapToDtoCollection());
    }

    [HttpGet]
    [Route("id")]
    [ProducesResponseType(typeof(SuggestedNameDto[]), 200)]
    public async Task<IActionResult> Get(string id)
    {
        var data = await _suggestedNameService.GetAsync(id);

        if(data == null)
            return NotFound($"Suggested name with id: {id} not found");


        return Ok(data.MapToDto());
    }

    [HttpDelete]
    [Route("id")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _suggestedNameService.DeleteSuggestedNameAsync(id);

        if(result)
        {
            return NoContent();
        }

        return BadRequest($"Suggested name with id: {id} not found as a suggested name");
    }
}
