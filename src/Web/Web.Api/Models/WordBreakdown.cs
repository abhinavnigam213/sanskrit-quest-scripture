using System.Text.Json.Serialization;
using SanskritQuest.Common.Contracts;

namespace SanskritQuest.Web.Api.Models;

public class WordBreakdown : ILocalizedTranslation
{
    [JsonPropertyName("sanskrit_word")]
    public string SanskritWord { get; set; } = string.Empty;

    [JsonPropertyName("transliteration")]
    public string Transliteration { get; set; } = string.Empty;

    [JsonPropertyName("english_meaning")]
    public string? TranslationEn { get; set; }

    [JsonPropertyName("hindi_meaning")]
    public string? TranslationHi { get; set; }
}
