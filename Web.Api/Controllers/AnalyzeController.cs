using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanskritQuest.Main.Web.Api.Models;
using SanskritQuest.Main.Web.Api.Services;

namespace SanskritQuest.Main.Web.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AnalyzeController : ControllerBase
{
    private readonly AIService _aiService;

    public AnalyzeController(AIService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost]
    public async Task<IActionResult> AnalyzeVerse([FromBody] AnalyzeRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Text))
        {
            return BadRequest(new { error = "Required field 'text' is missing." });
        }

        var result = await _aiService.AnalyzeScriptureAsync(
            request.Text, 
            request.SourceContext
        );

        return Ok(result);
    }
}
