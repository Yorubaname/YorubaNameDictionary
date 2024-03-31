using Application.Services;
using Microsoft.AspNetCore.Mvc;

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
}
