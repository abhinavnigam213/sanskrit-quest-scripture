using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanskritQuest.Main.Business.Contracts;

namespace SanskritQuest.Main.Web.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScripturesController : ControllerBase
{
    private readonly IScriptureService _scriptureService;

    public ScripturesController(IScriptureService scriptureService)
    {
        _scriptureService = scriptureService;
    }

    [HttpGet]
    public IActionResult GetPopularScriptures()
    {
        return Ok(_scriptureService.GetPopularScriptures());
    }
}
