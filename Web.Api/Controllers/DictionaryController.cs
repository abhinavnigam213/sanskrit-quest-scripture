using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanskritQuest.Main.Web.Api.Services;

namespace SanskritQuest.Main.Web.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DictionaryController : ControllerBase
{
    private readonly DataService _dataService;

    public DictionaryController(DataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet]
    public IActionResult SearchDictionary([FromQuery] string? word)
    {
        if (!string.IsNullOrEmpty(word))
        {
            string cleanWord = word.Trim().ToLower();

            if (cleanWord == "all" || cleanWord == "al")
            {
                return Ok(new Dictionary<string, object>
                {
                    ["Vedas"] = _dataService.VedasDict,
                    ["Upanishads"] = _dataService.UpanishadsDict,
                    ["Gita"] = _dataService.GitaDict,
                    ["Ramayana"] = _dataService.RamayanaDict,
                    ["Puranas"] = _dataService.PuranasDict,
                    ["all"] = _dataService.SpecializedDictionary
                });
            }

            // Exact match
            var exactMatch = _dataService.SpecializedDictionary.FirstOrDefault(
                kv => kv.Key.Equals(cleanWord, StringComparison.OrdinalIgnoreCase)
            );

            if (exactMatch.Key != null)
            {
                return Ok(new Dictionary<string, object>
                {
                    ["word"] = exactMatch.Key,
                    ["found"] = true,
                    ["entry"] = exactMatch.Value
                });
            }

            // Partial match
            var partialMatches = _dataService.SpecializedDictionary.Where(kv =>
                kv.Key.Contains(cleanWord, StringComparison.OrdinalIgnoreCase) ||
                kv.Value.Eng.Contains(cleanWord, StringComparison.OrdinalIgnoreCase) ||
                kv.Value.Hin.Contains(cleanWord, StringComparison.OrdinalIgnoreCase)
            ).ToDictionary(kv => kv.Key, kv => kv.Value);

            if (partialMatches.Count > 0)
            {
                return Ok(new Dictionary<string, object>
                {
                    ["word"] = word,
                    ["found"] = true,
                    ["message"] = $"Specific term not found, but found {partialMatches.Count} matching entry/entries.",
                    ["matches"] = partialMatches
                });
            }

            return NotFound(new Dictionary<string, object>
            {
                ["word"] = word,
                ["found"] = false,
                ["message"] = $"Word \"{word}\" not found in our specialized scriptures dictionary. Try querying \"all\" to retrieve all entries.",
                ["availableCategories"] = new[] { "Vedas", "Upanishads", "Gita", "Ramayana", "Puranas" }
            });
        }

        return Ok(new Dictionary<string, object>
        {
            ["Vedas"] = _dataService.VedasDict,
            ["Upanishads"] = _dataService.UpanishadsDict,
            ["Gita"] = _dataService.GitaDict,
            ["Ramayana"] = _dataService.RamayanaDict,
            ["Puranas"] = _dataService.PuranasDict,
            ["all"] = _dataService.SpecializedDictionary
        });
    }
}
