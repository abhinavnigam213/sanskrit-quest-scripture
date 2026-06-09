using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SanskritQuest.Data.Contracts;

namespace SanskritQuest.Business.Contracts;

public record TranslationRequest(
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("sourceLang")] string? SourceLang,
    [property: JsonPropertyName("targetLang")] string TargetLang,
    [property: JsonPropertyName("scriptureContext")] string? ScriptureContext
);

public record TranslationResponse(
    [property: JsonPropertyName("sourceLang")] string SourceLang,
    [property: JsonPropertyName("targetLang")] string TargetLang,
    [property: JsonPropertyName("translatedText")] string TranslatedText,
    [property: JsonPropertyName("explanation")] string Explanation,
    [property: JsonPropertyName("wordBreakdown")] List<WordBreakdownItem> WordBreakdown,
    [property: JsonPropertyName("isFallback")] bool? IsFallback
);

public record TransliterateRequest(
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("sourceScript")] string SourceScript,
    [property: JsonPropertyName("targetScript")] string TargetScript
);

public record TransliterateResponse(
    [property: JsonPropertyName("sourceScript")] string SourceScript,
    [property: JsonPropertyName("targetScript")] string TargetScript,
    [property: JsonPropertyName("transliteratedText")] string TransliteratedText
);

public record AnalyzeRequest(
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("sourceContext")] string? SourceContext
);
