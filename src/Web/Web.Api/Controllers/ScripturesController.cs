using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanskritQuest.Business.Contracts;
using SanskritQuest.Common.Contracts;
using SanskritQuest.Common.Utilities;
using SanskritQuest.Web.Api.Models;

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

	/// <summary>
	/// Retrieves popular scriptures with category metadata.
	/// </summary>
	/// <remarks>
	/// Sample Request:
	/// 
	///     GET /api/scriptures
	/// </remarks>
	[HttpGet]
	public IActionResult GetPopularScriptures()
	{
		var popular = _scriptureProvider.GetPopularScriptures();
		var translator = new Mapping.ApiTranslator();
		var response = popular.Select(s => translator.ScriptureBusinessToApi(s)).ToList();
		return Ok(response);
	}

	/// <summary>
	/// Retrieves all available scriptures with metadata and client-facing enums.
	/// </summary>
	/// <remarks>
	/// Sample Request:
	/// 
	///     GET /api/scriptures/all
	/// </remarks>
	[HttpGet("all")]
	public async Task<ActionResult<IEnumerable<ScriptureResponse>>> GetAllScriptures(CancellationToken cancellationToken)
	{
		var scriptures = await _scriptureProvider.GetAllScripturesAsync(cancellationToken);
		var translator = new Mapping.ApiTranslator();
		var response = scriptures.Select(s => translator.ScriptureBusinessToApi(s));
		return Ok(response);
	}

	/// <summary>
	/// Retrieves details of a scripture by its integer ID or by its ScriptureType enum name (e.g. BhagavadGita, ValmikiRamayana).
	/// </summary>
	/// <remarks>
	/// Sample Requests:
	/// 
	///     GET /api/scriptures/1
	///     (Retrieves using the integer database ID)
	/// 
	///     GET /api/scriptures/BhagavadGita
	///     (Retrieves using the ScriptureType enum name case-insensitively)
	/// </remarks>
	/// <param name="identifier">Scripture ID or ScriptureType enum name</param>
	/// <param name="cancellationToken">Cancellation token</param>
	[HttpGet("{identifier}")]
	public async Task<ActionResult<ScriptureDetailsResponse>> GetScriptureDetails(string identifier, CancellationToken cancellationToken)
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

		var details = await _scriptureProvider.GetScriptureDetailsAsync(scriptureId, cancellationToken);
		if (details == null)
		{
			return NotFound($"Scripture with identifier '{identifier}' was not found.");
		}

		var translator = new Mapping.ApiTranslator();
		var response = translator.ScriptureDetailsBusinessToApi(details);
		return Ok(response);
	}

	/// <summary>
	/// Retrieves detailed verse data including text, translations, transliteration, and word breakdown using a verse ID.
	/// </summary>
	/// <remarks>
	/// Sample Request:
	/// 
	///     GET /api/scriptures/verses/42
	/// </remarks>
	/// <param name="verseId">Integer ID of the verse</param>
	/// <param name="cancellationToken">Cancellation token</param>
	[HttpGet("verses/{verseId:int}")]
	public async Task<ActionResult<VerseDetailsResponse>> GetVerseDetails(int verseId, CancellationToken cancellationToken)
	{
		var details = await _scriptureProvider.GetVerseDetailsAsync(verseId, cancellationToken);
		if (details == null)
		{
			return NotFound($"Verse with ID '{verseId}' was not found.");
		}

		var translator = new Mapping.ApiTranslator();
		var response = translator.VerseDetailsBusinessToApi(details);
		return Ok(response);
	}

	/// <summary>
	/// Retrieves the details of a verse dynamically using its hierarchical path indices.
	/// </summary>
	/// <remarks>
	/// Sample Requests:
	/// 
	///     GET /api/scriptures/BG/verses/1/1
	///     (Retrieves Bhagavad Gita, Chapter 1, Shloka 1 using path)
	/// 
	///     GET /api/scriptures/BG/verses?path=8/3
	///     (Retrieves using query parameter, ideal for Swagger UI)
	/// </remarks>
	/// <param name="scriptureCode">The scripture source code (e.g., BG, VR)</param>
	/// <param name="hierarchyPath">Slash-separated hierarchical indices in path (e.g., 1/1 for BG)</param>
	/// <param name="path">Slash-separated hierarchical indices in query (e.g., 8/3 for BG)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	[HttpGet("{scriptureCode}/verses")]
	[HttpGet("{scriptureCode}/verses/{**hierarchyPath}")]
	public async Task<ActionResult<VerseDetailsResponse>> GetVerseDetailsByHierarchy(
		string scriptureCode, 
		string? hierarchyPath, 
		[FromQuery] string? path, 
		CancellationToken cancellationToken)
	{
		var resolvedPath = hierarchyPath ?? path;
		if (string.IsNullOrWhiteSpace(resolvedPath))
		{
			return BadRequest("Hierarchy path cannot be empty. Provide it in the path or as a '?path=' query parameter.");
		}

		var decodedPath = System.Net.WebUtility.UrlDecode(resolvedPath);
		var segments = decodedPath.Split('/')
			.Select(s => int.TryParse(s, out var val) ? val : (int?)null)
			.ToList();

		if (segments.Any(s => s == null))
		{
			return BadRequest("Hierarchy levels must be valid integers.");
		}

		var levelNumbers = segments.Cast<int>().ToArray();
		var details = await _scriptureProvider.GetVerseDetailsByHierarchyAsync(scriptureCode, levelNumbers, cancellationToken);
		if (details == null)
		{
			return NotFound($"Verse with hierarchy '{resolvedPath}' under scripture '{scriptureCode}' was not found.");
		}

		var translator = new Mapping.ApiTranslator();
		var response = translator.VerseDetailsBusinessToApi(details);
		return Ok(response);
	}
}
