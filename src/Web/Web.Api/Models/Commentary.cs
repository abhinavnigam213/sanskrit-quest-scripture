using System.Text.Json.Serialization;

namespace SanskritQuest.Web.Api.Models;

public class Commentary
{
    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;

    [JsonPropertyName("english_commentary")]
    public string EnglishCommentary { get; set; } = string.Empty;

    [JsonPropertyName("hindi_commentary")]
    public string HindiCommentary { get; set; } = string.Empty;
}
