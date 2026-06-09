using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SanskritQuest.Business.Contracts;
using SanskritQuest.Data.Contracts;
using SanskritQuest.Data.Providers;
using SanskritQuest.Services.AIService;

namespace SanskritQuest.Business.Providers;

public class ScriptureService : IScriptureService
{
    private readonly DataService _dataService;

    public ScriptureService(DataService dataService)
    {
        _dataService = dataService;
    }

    public List<Scripture> GetPopularScriptures()
    {
        return _dataService.PopularScriptures;
    }
}

public class DictionaryService : IDictionaryService
{
    private readonly DataService _dataService;

    public DictionaryService(DataService dataService)
    {
        _dataService = dataService;
    }

    public object GetDictionaryData(string? word)
    {
        if (!string.IsNullOrEmpty(word))
        {
            string cleanWord = word.Trim().ToLower();

            if (cleanWord == "all" || cleanWord == "al")
            {
                return new Dictionary<string, object>
                {
                    ["Vedas"] = _dataService.VedasDict,
                    ["Upanishads"] = _dataService.UpanishadsDict,
                    ["Gita"] = _dataService.GitaDict,
                    ["Ramayana"] = _dataService.RamayanaDict,
                    ["Puranas"] = _dataService.PuranasDict,
                    ["all"] = _dataService.SpecializedDictionary
                };
            }

            // Exact match
            var exactMatch = _dataService.SpecializedDictionary.FirstOrDefault(
                kv => kv.Key.Equals(cleanWord, StringComparison.OrdinalIgnoreCase)
            );

            if (exactMatch.Key != null)
            {
                return new Dictionary<string, object>
                {
                    ["word"] = exactMatch.Key,
                    ["found"] = true,
                    ["entry"] = exactMatch.Value
                };
            }

            // Partial match
            var partialMatches = _dataService.SpecializedDictionary.Where(kv =>
                kv.Key.Contains(cleanWord, StringComparison.OrdinalIgnoreCase) ||
                kv.Value.Eng.Contains(cleanWord, StringComparison.OrdinalIgnoreCase) ||
                kv.Value.Hin.Contains(cleanWord, StringComparison.OrdinalIgnoreCase)
            ).ToDictionary(kv => kv.Key, kv => kv.Value);

            if (partialMatches.Count > 0)
            {
                return new Dictionary<string, object>
                {
                    ["word"] = word,
                    ["found"] = true,
                    ["message"] = $"Specific term not found, but found {partialMatches.Count} matching entry/entries.",
                    ["matches"] = partialMatches
                };
            }

            return new Dictionary<string, object>
            {
                ["word"] = word,
                ["found"] = false,
                ["message"] = $"Word \"{word}\" not found in our specialized scriptures dictionary. Try querying \"all\" to retrieve all entries.",
                ["availableCategories"] = new[] { "Vedas", "Upanishads", "Gita", "Ramayana", "Puranas" }
            };
        }

        return new Dictionary<string, object>
        {
            ["Vedas"] = _dataService.VedasDict,
            ["Upanishads"] = _dataService.UpanishadsDict,
            ["Gita"] = _dataService.GitaDict,
            ["Ramayana"] = _dataService.RamayanaDict,
            ["Puranas"] = _dataService.PuranasDict,
            ["all"] = _dataService.SpecializedDictionary
        };
    }
}

public class TranslationService : ITranslationService
{
    private readonly AIService _aiService;

    public TranslationService(AIService aiService)
    {
        _aiService = aiService;
    }

    public Task<TranslationResponse> TranslateTextAsync(TranslationRequest request)
    {
        return _aiService.TranslateTextAsync(
            request.Text,
            request.SourceLang ?? "auto",
            request.TargetLang,
            request.ScriptureContext
        );
    }
}

public class TransliterateService : ITransliterateService
{
    private readonly AIService _aiService;

    public TransliterateService(AIService aiService)
    {
        _aiService = aiService;
    }

    public Task<TransliterateResponse> TransliterateTextAsync(TransliterateRequest request)
    {
        return _aiService.TransliterateTextAsync(
            request.Text,
            request.SourceScript,
            request.TargetScript
        );
    }
}

public class AnalyzeService : IAnalyzeService
{
    private readonly AIService _aiService;

    public AnalyzeService(AIService aiService)
    {
        _aiService = aiService;
    }

    public Task<ScriptureAnalyzeResponse> AnalyzeScriptureAsync(AnalyzeRequest request)
    {
        return _aiService.AnalyzeScriptureAsync(
            request.Text,
            request.SourceContext
        );
    }
}
