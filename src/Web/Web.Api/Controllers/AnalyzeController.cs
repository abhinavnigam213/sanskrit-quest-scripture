using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanskritQuest.Main.Business.Contracts;

namespace SanskritQuest.Main.Web.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AnalyzeController : ControllerBase
{
    private readonly IAnalyzeService _analyzeService;

    public AnalyzeController(IAnalyzeService analyzeService)
    {
        _analyzeService = analyzeService;
    }

    [HttpPost]
    public async Task<IActionResult> AnalyzeVerse([FromBody] AnalyzeRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Text))
        {
            return BadRequest(new { error = "Required field 'text' is missing." });
        }

        var result = await _analyzeService.AnalyzeScriptureAsync(request);

        return Ok(result);
    }
}
