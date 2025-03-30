using Api.Utilities;
using Core.Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FluentValidation;
using Application.Services.Names;
using Core.Dto.Response;
using Application.Mappers.Names;

namespace Api.Controllers.Names;

[Route("api/v1/suggestions")]
[ApiController]
[Authorize(Policy = "AdminAndLexicographers")]
public class SuggestedNameController : ControllerBase
{
    private readonly SuggestedNameService _suggestedNameService;
    private readonly IValidator<CreateSuggestedNameDto> _suggestedNameValidator;

    public SuggestedNameController(SuggestedNameService suggestedNameService, IValidator<CreateSuggestedNameDto> suggestedNameValidator)
    {
        _suggestedNameService = suggestedNameService;
        _suggestedNameValidator = suggestedNameValidator;
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> Create([FromBody] CreateSuggestedNameDto request)
    {

        var result = await _suggestedNameValidator.ValidateAsync(request);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }
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
