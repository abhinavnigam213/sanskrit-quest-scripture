using System.Text.Json.Serialization;
using SanskritQuest.Common.Contracts;

namespace SanskritQuest.Web.Api.Models;

public class Translation : ILocalizedTranslation
{
    [JsonPropertyName("english")]
    public string? TranslationEn { get; set; }

    [JsonPropertyName("hindi")]
    public string? TranslationHi { get; set; }
}
