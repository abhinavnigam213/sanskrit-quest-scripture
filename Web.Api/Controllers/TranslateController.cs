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
public class TranslateController : ControllerBase
{
    private readonly AIService _aiService;

    public TranslateController(AIService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost]
    public async Task<IActionResult> TranslateText([FromBody] TranslationRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Text) || string.IsNullOrEmpty(request.TargetLang))
        {
            return BadRequest(new { error = "Required fields 'text' and 'targetLang' are missing." });
        }

        var result = await _aiService.TranslateTextAsync(
            request.Text, 
            request.SourceLang ?? "auto", 
            request.TargetLang, 
            request.ScriptureContext
        );

        return Ok(result);
    }
}
