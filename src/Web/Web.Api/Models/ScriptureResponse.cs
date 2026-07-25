using System.Collections.Generic;
using System.Text.Json.Serialization;
using SanskritQuest.Common.Contracts;

namespace SanskritQuest.Web.Api.Models;

public class ScriptureResponse
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

    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }

    [JsonPropertyName("categoryName")]
    public string CategoryName { get; set; } = string.Empty;

    [JsonPropertyName("classId")]
    public int ClassId { get; set; }

    [JsonPropertyName("className")]
    public string ClassName { get; set; } = string.Empty;

    [JsonPropertyName("sourceId")]
    public int SourceId { get; set; }

    [JsonPropertyName("searchWeight")]
    public int SearchWeight { get; set; }

    [JsonPropertyName("metaTags")]
    public List<string> MetaTags { get; set; } = new();

    [JsonPropertyName("enumName")]
    public string EnumName { get; set; } = string.Empty;
}
