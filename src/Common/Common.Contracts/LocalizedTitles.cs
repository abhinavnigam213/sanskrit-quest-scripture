using System.Text.Json.Serialization;

namespace SanskritQuest.Common.Contracts
{
	public class LocalizedTitles
	{
		[JsonPropertyName("en")]
		public string En { get; set; } = string.Empty;

		[JsonPropertyName("hi")]
		public string Hi { get; set; } = string.Empty;

		[JsonPropertyName("sa")]
		public string Sa { get; set; } = string.Empty;
	}
}
