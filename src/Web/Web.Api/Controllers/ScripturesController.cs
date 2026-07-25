using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanskritQuest.Common.Contracts;
using SanskritQuest.Common.Utilities;
using SanskritQuest.Web.Api.MockBusiness;
using SanskritQuest.Web.Api.Models;

namespace SanskritQuest.Web.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScripturesController : ControllerBase
{
	// TODO: Replace IMockScriptureProvider with IScriptureProvider once the actual business layer is implemented.
	private readonly IMockScriptureProvider _mockScriptureProvider;

	public ScripturesController(IMockScriptureProvider mockScriptureProvider)
	{
		_mockScriptureProvider = mockScriptureProvider;
	}

	[HttpGet]
	public IActionResult GetPopularScriptures()
	{
		return Ok(_mockScriptureProvider.GetPopularScriptures());
	}

	/// <summary>
	/// Retrieves all available scriptures with metadata and client-facing enums.
	/// </summary>
	[HttpGet("all")]
	public ActionResult<IEnumerable<ScriptureResponse>> GetAllScriptures()
	{
		return Ok(_mockScriptureProvider.GetAllScriptures());
	}

	/// <summary>
	/// Retrieves details of a scripture by its integer ID or by its ScriptureType enum name (e.g. BhagavadGita, ValmikiRamayana).
	/// </summary>
	/// <param name="identifier">Scripture ID or ScriptureType enum name</param>
	[HttpGet("{identifier}")]
	public ActionResult<ScriptureDetailsResponse> GetScriptureDetails(string identifier)
	{
		int scriptureId;
		if (int.TryParse(identifier, out int id))
		{
			scriptureId = id;
		}
		else if (Enum.TryParse<ScriptureType>(identifier, true, out var scriptureType))
		{
			var dbId = scriptureType.GetDatabaseId();
			if (!dbId.HasValue)
			{
				return BadRequest($"Scripture type '{identifier}' is not mapped to a database ID.");
			}
			scriptureId = dbId.Value;
		}
		else
		{
			return BadRequest($"Invalid scripture identifier: '{identifier}'. Must be a valid integer ID or a valid ScriptureType enum name (e.g. BhagavadGita, ValmikiRamayana).");
		}

		var details = _mockScriptureProvider.GetScriptureDetails(scriptureId);
		if (details == null)
		{
			return NotFound($"Scripture with identifier '{identifier}' was not found.");
		}

		return Ok(details);
	}

	[HttpGet("verses/{verseId:int}")]
	public ActionResult<VerseDetailsResponse> GetVerseDetails(int verseId)
	{
		return Ok(_mockScriptureProvider.GetVerseDetails("BG", new[] { 1, 1 }));
	}

	[HttpGet("{scriptureCode}/verses/{**hierarchyPath}")]
	public ActionResult<VerseDetailsResponse> GetVerseDetailsByHierarchy(string scriptureCode, string hierarchyPath)
	{
		if (string.IsNullOrWhiteSpace(hierarchyPath))
		{
			return BadRequest("Hierarchy path cannot be empty.");
		}

		var segments = hierarchyPath.Split('/')
			.Select(s => int.TryParse(s, out var val) ? val : (int?)null)
			.ToList();

		if (segments.Any(s => s == null))
		{
			return BadRequest("Hierarchy levels must be valid integers.");
		}

		var levelNumbers = segments.Cast<int>().ToArray();
		return Ok(_mockScriptureProvider.GetVerseDetails(scriptureCode, levelNumbers));
	}
}
