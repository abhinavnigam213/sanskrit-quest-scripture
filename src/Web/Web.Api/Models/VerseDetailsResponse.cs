using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SanskritQuest.Web.Api.Models;

public class VerseDetailsResponse
{
    [JsonPropertyName("scripture_name")]
    public string ScriptureName { get; set; } = string.Empty;

    [JsonPropertyName("hierarchy")]
    public List<HierarchyLevel> Hierarchy { get; set; } = new();

    [JsonPropertyName("sanskrit_shloka")]
    public string SanskritShloka { get; set; } = string.Empty;

    [JsonPropertyName("transliteration")]
    public string Transliteration { get; set; } = string.Empty;

    [JsonPropertyName("word_by_word_breakdown")]
    public List<WordBreakdown> WordByWordBreakdown { get; set; } = new();

    [JsonPropertyName("translation")]
    public Translation Translation { get; set; } = new();

    [JsonPropertyName("commentaries")]
    public List<Commentary> Commentaries { get; set; } = new();
}
