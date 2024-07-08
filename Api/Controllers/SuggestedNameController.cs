using Api.Utilities;
using Application.Services;
using Core.Dto.Request;
using Core.Dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Application.Mappers;

namespace Api.Controllers;

[Route("api/v1/suggestions")]
[ApiController]
[Authorize(Policy = "AdminAndLexicographers")]
public class SuggestedNameController : ControllerBase
{
    private readonly SuggestedNameService _suggestedNameService;

    public SuggestedNameController(SuggestedNameService suggestedNameService)
    {
        _suggestedNameService = suggestedNameService;
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> Create([FromBody] CreateSuggestedNameDto request)
    {
        await _suggestedNameService
                .CreateAsync(request.MapToEntity());

        return StatusCode((int)HttpStatusCode.Created, ResponseHelper.GetResponseDict("Suggested Name successfully added"));
    }

    [HttpGet]
    [Route("meta")]
    [ProducesResponseType(typeof(Dictionary<string, int>), 200)]
    public async Task<IActionResult> GetMetaData()
    {
        var suggestname = await _suggestedNameService.CountAsync();

        return Ok(suggestname);
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(SuggestedNameDto[]), 200)]
    public async Task<IActionResult> GetAll()
    {
        var data = await _suggestedNameService.GetAllAsync();
        return Ok(data.MapToDtoCollection());
    }

    [HttpDelete]
    [Authorize(Policy = "AdminAndProLexicographers")]
    [Route("{id}")]
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
