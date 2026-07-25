using System.Text.Json.Serialization;

namespace SanskritQuest.Web.Api.Models;

public class HierarchyLevel
{
    [JsonPropertyName("level_type")]
    public string LevelType { get; set; } = string.Empty;

    [JsonPropertyName("level_number")]
    public int LevelNumber { get; set; }

    [JsonPropertyName("level_name_sanskrit")]
    public string? LevelNameSanskrit { get; set; }
}
