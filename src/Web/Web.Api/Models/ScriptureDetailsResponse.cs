using System.Collections.Generic;
using System.Text.Json.Serialization;
using SanskritQuest.Common.Contracts;

namespace SanskritQuest.Web.Api.Models;

public class ScriptureDetailsResponse
{
    [JsonPropertyName("scriptureId")]
    public int ScriptureId { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("titles")]
    public LocalizedTitles Titles { get; set; } = new();

    [JsonPropertyName("author")]
    public LocalizedDescription? Author { get; set; }

    [JsonPropertyName("description")]
    public LocalizedDescription Description { get; set; } = new();

    [JsonPropertyName("enumName")]
    public string EnumName { get; set; } = string.Empty;

    [JsonPropertyName("hierarchy")]
    public List<ScriptureHierarchyNodeResponse> Hierarchy { get; set; } = new();
}
