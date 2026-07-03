using Insight.Database;

namespace SanskritQuest.Data.Contracts
{
	public class DictionaryRecord
	{
		public long Id { get; set; }

		[Column("slp1")]
		public string Slp1 { get; set; } = string.Empty;

		[Column("iast")]
		public string Iast { get; set; } = string.Empty;

		[Column("devanagari")]
		public string Devanagari { get; set; } = string.Empty;

		[Column("source")]
		public string Source { get; set; } = string.Empty;

		[Column("english_meanings")]
		public string EnglishMeanings { get; set; } = string.Empty;

		[Column("hindi_meanings")]
		public string HindiMeanings { get; set; } = string.Empty;

		[Column("lexical_info")]
		public string LexicalInfo { get; set; } = string.Empty;

		[Column("raw_xml")]
		public string RawXml { get; set; } = string.Empty;
	}
}
