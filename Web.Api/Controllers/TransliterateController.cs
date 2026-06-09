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
public class TransliterateController : ControllerBase
{
    private readonly AIService _aiService;

    public TransliterateController(AIService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost]
    public async Task<IActionResult> TransliterateText([FromBody] TransliterateRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Text) || string.IsNullOrEmpty(request.SourceScript) || string.IsNullOrEmpty(request.TargetScript))
        {
            return BadRequest(new { error = "Required fields 'text', 'sourceScript', and 'targetScript' are missing." });
        }

        var result = await _aiService.TransliterateTextAsync(
            request.Text, 
            request.SourceScript, 
            request.TargetScript
        );

        return Ok(result);
    }
}
