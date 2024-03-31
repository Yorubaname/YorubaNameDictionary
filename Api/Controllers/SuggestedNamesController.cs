using Api.Mappers;
using Application.Services;
using Core.Dto.Request;
using Core.Dto.Response;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SuggestedNamesController : ControllerBase
{
    private readonly SuggestedNameService _suggestedNameService;

    public SuggestedNamesController(SuggestedNameService suggestedNameService)
    {
        _suggestedNameService = suggestedNameService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSuggestedMetaData()
    {
        var suggestname = await _suggestedNameService.CountAsync();

        return Ok(suggestname);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SuggestedNameDto[]), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SuggestName([FromBody] CreateSuggestedNameDto request)
    {
        var data = await _suggestedNameService.SuggestedNameAsync(request.MapToEntity());
        return Ok(data.MapToDto());
    }
}
