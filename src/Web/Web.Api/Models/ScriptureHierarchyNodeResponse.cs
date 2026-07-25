using System.Text.Json.Serialization;
using SanskritQuest.Common.Contracts;

namespace SanskritQuest.Web.Api.Models;

public class ScriptureHierarchyNodeResponse
{
    [JsonPropertyName("hierarchyId")]
    public int HierarchyId { get; set; }

    [JsonPropertyName("parentId")]
    public int? ParentId { get; set; }

    [JsonPropertyName("localLabel")]
    public string LocalLabel { get; set; } = string.Empty;

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("nodeType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public HierarchyNodeType NodeType { get; set; }

    [JsonPropertyName("titles")]
    public LocalizedTitles Titles { get; set; } = new();

    [JsonPropertyName("description")]
    public LocalizedDescription Description { get; set; } = new();

    [JsonPropertyName("sequenceNumber")]
    public int SequenceNumber { get; set; }

    [JsonPropertyName("directVerseCount")]
    public int DirectVerseCount { get; set; }

    [JsonPropertyName("recursiveVerseCount")]
    public int RecursiveVerseCount { get; set; }

    [JsonPropertyName("fullHierarchyLabel")]
    public string FullHierarchyLabel { get; set; } = string.Empty;

    [JsonPropertyName("children")]
    public System.Collections.Generic.List<ScriptureHierarchyNodeResponse> Children { get; set; } = new();
}
