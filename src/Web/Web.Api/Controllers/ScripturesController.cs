using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanskritQuest.Business.Contracts;

namespace SanskritQuest.Web.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScripturesController : ControllerBase
{
    private readonly IScriptureProvider _scriptureProvider;

    public ScripturesController(IScriptureProvider scriptureProvider)
    {
        _scriptureProvider = scriptureProvider;
    }

    [HttpGet]
    public IActionResult GetPopularScriptures()
    {
        return Ok(_scriptureProvider.GetPopularScriptures());
    }
}
