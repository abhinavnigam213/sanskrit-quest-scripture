using Insight.Database;

namespace SanskritQuest.Data.Contracts
{
	public class VerseIdForHierarchy
	{
		[Column("verse_id")]
		public int VerseId { get; set; }

		[Column("content_sanskrit")]
		public string ContentSanskrit { get; set; } = string.Empty;
	}
}
