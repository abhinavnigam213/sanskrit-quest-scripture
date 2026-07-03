using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanskritQuest.Business.Contracts;

namespace SanskritQuest.Web.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LanguageController : ControllerBase
{
	private readonly ILanguageProvider _languageProvider;

	public LanguageController(ILanguageProvider languageProvider)
	{
		_languageProvider = languageProvider;
	}

	[HttpPost("/api/analyze")]
	public async Task<IActionResult> AnalyzeVerse([FromBody] AnalyzeRequest request)
	{
		if (request == null || string.IsNullOrEmpty(request.Text))
		{
			return BadRequest(new { error = "Required field 'text' is missing." });
		}

		var result = await _languageProvider.AnalyzeScriptureAsync(request);

		return Ok(result);
	}

	[HttpPost("/api/translate")]
	public async Task<IActionResult> TranslateText([FromBody] TranslationRequest request)
	{
		if (request == null || string.IsNullOrEmpty(request.Text) || string.IsNullOrEmpty(request.TargetLang))
		{
			return BadRequest(new { error = "Required fields 'text' and 'targetLang' are missing." });
		}

		var result = await _languageProvider.TranslateTextAsync(request);

		return Ok(result);
	}

	[HttpPost("/api/transliterate")]
	public async Task<IActionResult> TransliterateText([FromBody] TransliterateRequest request)
	{
		if (request == null || string.IsNullOrEmpty(request.Text) || string.IsNullOrEmpty(request.SourceScript) || string.IsNullOrEmpty(request.TargetScript))
		{
			return BadRequest(new { error = "Required fields 'text', 'sourceScript', and 'targetScript' are missing." });
		}

		var result = await _languageProvider.TransliterateTextAsync(request);

		return Ok(result);
	}
}
