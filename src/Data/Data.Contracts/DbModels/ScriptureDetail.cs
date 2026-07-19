using Insight.Database;
using SanskritQuest.Common.Contracts;

namespace SanskritQuest.Data.Contracts
{
	public class ScriptureDetail
	{
		[Column("scripture_id")]
		public int ScriptureId { get; set; }

		[Column("scripture_code")]
		public string ScriptureCode { get; set; } = string.Empty;

		[Column("scripture_author", SerializationMode = SerializationMode.Json)]
		public LocalizedDescription? ScriptureAuthor { get; set; }

		[Column("scripture_titles", SerializationMode = SerializationMode.Json)]
		public LocalizedTitles ScriptureTitles { get; set; } = new();

		[Column("scripture_description", SerializationMode = SerializationMode.Json)]
		public LocalizedDescription ScriptureDescription { get; set; } = new();

		[Column("hierarchy_id")]
		public int HierarchyId { get; set; }

		[Column("parent_id")]
		public int? ParentId { get; set; }

		[Column("local_label")]
		public string LocalLabel { get; set; } = string.Empty;

		[Column("path")]
		public string Path { get; set; } = string.Empty;

		[Column("node_type")]
		public HierarchyNodeType NodeType { get; set; }

		[Column("hierarchy_titles", SerializationMode = SerializationMode.Json)]
		public LocalizedTitles HierarchyTitles { get; set; } = new();

		[Column("hierarchy_description", SerializationMode = SerializationMode.Json)]
		public LocalizedDescription HierarchyDescription { get; set; } = new();

		[Column("sequence_number")]
		public int SequenceNumber { get; set; }

		[Column("direct_verse_count")]
		public int DirectVerseCount { get; set; }

		[Column("recursive_verse_count")]
		public int RecursiveVerseCount { get; set; }
	}
}
