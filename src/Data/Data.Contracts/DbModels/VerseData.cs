using System.Collections.Generic;
using System.Text.Json.Serialization;
using SanskritQuest.Common.Contracts;

namespace SanskritQuest.Data.Contracts
{
	public class VerseData : ILocalizedTranslation
	{
		[JsonPropertyName("translation_en")]
		public string? TranslationEn { get; set; }

		[JsonPropertyName("translation_hi")]
		public string? TranslationHi { get; set; }

		[JsonPropertyName("word_by_word_breakdown")]
		public List<DbWordBreakdownItem> WordByWordBreakdown { get; set; } = new();
	}
}
