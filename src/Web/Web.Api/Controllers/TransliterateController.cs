using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanskritQuest.Main.Business.Contracts;

namespace SanskritQuest.Main.Web.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransliterateController : ControllerBase
{
    private readonly ITransliterateService _transliterateService;

    public TransliterateController(ITransliterateService transliterateService)
    {
        _transliterateService = transliterateService;
    }

    [HttpPost]
    public async Task<IActionResult> TransliterateText([FromBody] TransliterateRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Text) || string.IsNullOrEmpty(request.SourceScript) || string.IsNullOrEmpty(request.TargetScript))
        {
            return BadRequest(new { error = "Required fields 'text', 'sourceScript', and 'targetScript' are missing." });
        }

        var result = await _transliterateService.TransliterateTextAsync(request);

        return Ok(result);
    }
}
