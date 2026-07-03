using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SanskritQuest.Data.Contracts;

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

public record GenericWordDetails(
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
