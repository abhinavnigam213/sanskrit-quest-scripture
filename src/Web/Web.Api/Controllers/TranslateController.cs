using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanskritQuest.Main.Business.Contracts;

namespace SanskritQuest.Main.Web.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TranslateController : ControllerBase
{
    private readonly ITranslationService _translationService;

    public TranslateController(ITranslationService translationService)
    {
        _translationService = translationService;
    }

    [HttpPost]
    public async Task<IActionResult> TranslateText([FromBody] TranslationRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Text) || string.IsNullOrEmpty(request.TargetLang))
        {
            return BadRequest(new { error = "Required fields 'text' and 'targetLang' are missing." });
        }

        var result = await _translationService.TranslateTextAsync(request);

        return Ok(result);
    }
}
