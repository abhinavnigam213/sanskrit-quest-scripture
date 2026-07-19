using System.Collections.Generic;
using Insight.Database;
using SanskritQuest.Common.Contracts;

namespace SanskritQuest.Data.Contracts
{
	public class VerseDetail
	{
		[Column("verse_id")]
		public int VerseId { get; set; }

		[Column("verse_number")]
		public string VerseNumber { get; set; } = string.Empty;

		[Column("verse_type")]
		public VerseType VerseType { get; set; }

		[Column("content_sanskrit")]
		public string ContentSanskrit { get; set; } = string.Empty;

		[Column("verse_data", SerializationMode = SerializationMode.Json)]
		public VerseData VerseData { get; set; } = new();

		[Column("word_by_word_breakdown", SerializationMode = SerializationMode.Json)]
		public List<DbWordBreakdownItem> WordByWordBreakdown { get; set; } = new();

		[Column("source_id")]
		public int SourceId { get; set; }

		[Column("search_weight")]
		public int SearchWeight { get; set; }

		[Column("verse_meta_tags", SerializationMode = SerializationMode.Json)]
		public List<string> VerseMetaTags { get; set; } = new();

		[Column("hierarchy_id")]
		public int HierarchyId { get; set; }

		[Column("hierarchy_parent_id")]
		public int? HierarchyParentId { get; set; }

		[Column("hierarchy_local_label")]
		public string HierarchyLocalLabel { get; set; } = string.Empty;

		[Column("hierarchy_path")]
		public string HierarchyPath { get; set; } = string.Empty;

		[Column("hierarchy_node_type")]
		public HierarchyNodeType HierarchyNodeType { get; set; }

		[Column("hierarchy_titles", SerializationMode = SerializationMode.Json)]
		public LocalizedTitles HierarchyTitles { get; set; } = new();

		[Column("hierarchy_description", SerializationMode = SerializationMode.Json)]
		public LocalizedDescription HierarchyDescription { get; set; } = new();

		[Column("hierarchy_meta_tags", SerializationMode = SerializationMode.Json)]
		public List<string> HierarchyMetaTags { get; set; } = new();

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

		[Column("scripture_meta_tags", SerializationMode = SerializationMode.Json)]
		public List<string> ScriptureMetaTags { get; set; } = new();
	}
}
