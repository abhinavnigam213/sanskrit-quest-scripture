using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanskritQuest.Business.Contracts;

namespace SanskritQuest.Web.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DictionaryController : ControllerBase
{
    private readonly IDictionaryService _dictionaryService;

    public DictionaryController(IDictionaryService dictionaryService)
    {
        _dictionaryService = dictionaryService;
    }

    [HttpGet]
    public IActionResult SearchDictionary([FromQuery] string? word)
    {
        var result = _dictionaryService.GetDictionaryData(word);
        
        // If the service result is a dictionary search failure response
        if (result is Dictionary<string, object> dict && dict.TryGetValue("found", out var found) && found is false)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
