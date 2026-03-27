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

[Route("api/v1/[controller]")]
[ApiController]
[Authorize(Policy = "AdminAndLexicographers")]
public class SuggestionsController : ControllerBase
{
    private readonly SuggestionsService _suggestedNameService;
    private readonly IValidator<CreateSuggestedNameDto> _suggestedNameValidator;

    public SuggestionsController(SuggestionsService suggestedNameService, IValidator<CreateSuggestedNameDto> suggestedNameValidator)
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

    [HttpDelete]
    [Authorize(Policy = "AdminAndProLexicographers")]
    [Route("batch")]
    public async Task<IActionResult> DeleteBatch([FromBody] string[] names)
    {
        if (names is null || names.Length == 0)
        {
            return BadRequest("No deletion as no names were provided");
        }

        var deleteResult = await _suggestedNameService.DeleteSuggestedNamesBatchAsync(names);

        if (deleteResult.DeletedItems.Length == 0)
        {
            return BadRequest("No deletion as none of the names were found as suggestions in the database");
        }

        string responseMessage = string.Join(", ", deleteResult.DeletedItems) + " deleted";
        if (deleteResult.NotFoundItems.Length > 0)
        {
            responseMessage += Environment.NewLine + string.Join(", ", deleteResult.NotFoundItems) + " not deleted as they were not found in suggested names";
        }

        return Ok(new { Message = responseMessage });
    }
}
