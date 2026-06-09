using System.Text.Json.Serialization;

namespace SanskritQuest.Main.Web.Api.Models;

public record Scripture(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("source")] string Source,
    [property: JsonPropertyName("category")] string Category,
    [property: JsonPropertyName("verse")] string Verse,
    [property: JsonPropertyName("transliterationDefault")] string TransliterationDefault,
    [property: JsonPropertyName("translationDefaultEnglish")] string TranslationDefaultEnglish,
    [property: JsonPropertyName("translationDefaultHindi")] string TranslationDefaultHindi
);

public record DictionaryEntry(
    [property: JsonPropertyName("category")] string Category,
    [property: JsonPropertyName("grammar")] string Grammar,
    [property: JsonPropertyName("eng")] string Eng,
    [property: JsonPropertyName("hin")] string Hin
);

public record WordBreakdownItem(
    [property: JsonPropertyName("word")] string Word,
    [property: JsonPropertyName("grammar")] string? Grammar,
    [property: JsonPropertyName("meaningHindi")] string MeaningHindi,
    [property: JsonPropertyName("meaningEnglish")] string MeaningEnglish
);

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

public record ScriptureAnalyzeResponse(
    [property: JsonPropertyName("verse")] string Verse,
    [property: JsonPropertyName("identifiedSource")] string? IdentifiedSource,
    [property: JsonPropertyName("transliterationIAST")] string TransliterationIAST,
    [property: JsonPropertyName("transliterationPhonetic")] string TransliterationPhonetic,
    [property: JsonPropertyName("translationEnglish")] string TranslationEnglish,
    [property: JsonPropertyName("translationHindi")] string TranslationHindi,
    [property: JsonPropertyName("spiritualSignificance")] string SpiritualSignificance,
    [property: JsonPropertyName("poeticMeter")] string? PoeticMeter,
    [property: JsonPropertyName("wordBreakdown")] List<WordBreakdownItem> WordBreakdown,
    [property: JsonPropertyName("isFallback")] bool? IsFallback
);
