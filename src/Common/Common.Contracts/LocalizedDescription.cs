using System.Text.Json.Serialization;

namespace SanskritQuest.Common.Contracts
{
	public class LocalizedDescription
	{
		[JsonPropertyName("en")]
		public string? En { get; set; }

		[JsonPropertyName("hi")]
		public string? Hi { get; set; }
	}
}
