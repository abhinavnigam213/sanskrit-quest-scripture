using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanskritQuest.Business.Contracts;

namespace SanskritQuest.Web.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DictionaryController : ControllerBase
{
	private readonly IDictionaryProvider _dictionaryProvider;

	public DictionaryController(IDictionaryProvider dictionaryProvider)
	{
		_dictionaryProvider = dictionaryProvider;
	}

	[HttpGet]
	public IActionResult GetDictionary([FromQuery] string? word = null)
	{
		var result = !string.IsNullOrEmpty(word)
			? _dictionaryProvider.SearchDictionary(word)
			: _dictionaryProvider.GetAllDictionaryData();

		// If the service result is a dictionary search failure response
		if (result is Dictionary<string, object> dict && dict.TryGetValue("found", out var found) && found is false)
		{
			return NotFound(result);
		}

		return Ok(result);
	}
}
