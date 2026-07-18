using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Insight.Database;

namespace SanskritQuest.Data.Contracts
{
	public enum HierarchyNodeType
	{
		Scripture,
		Kanda,
		Adhyaya,
		Sarga,
		Chapter
	}

	public enum VerseType
	{
		Shloka,
		Mantra,
		Stotra_Verse,
		Stotra
	}

	public enum TranslationLanguage
	{
		English,
		Hindi,
		Both
	}

	public class LocalizedTitles
	{
		[JsonPropertyName("en")]
		public string En { get; set; } = string.Empty;

		[JsonPropertyName("hi")]
		public string Hi { get; set; } = string.Empty;

		[JsonPropertyName("sa")]
		public string Sa { get; set; } = string.Empty;
	}

	public class LocalizedDescription
	{
		[JsonPropertyName("en")]
		public string? En { get; set; }

		[JsonPropertyName("hi")]
		public string? Hi { get; set; }
	}

	public class VerseData
	{
		[JsonPropertyName("translation_en")]
		public string? TranslationEn { get; set; }

		[JsonPropertyName("translation_hi")]
		public string? TranslationHi { get; set; }

		[JsonPropertyName("word_breakdown")]
		public List<WordBreakdownItem> WordBreakdown { get; set; } = new();
	}

	public class DbScripture
	{
		[Column("scripture_id")]
		public int ScriptureId { get; set; }

		[Column("code")]
		public string Code { get; set; } = string.Empty;

		[Column("title_en")]
		public string TitleEn { get; set; } = string.Empty;

		[Column("title_hi")]
		public string TitleHi { get; set; } = string.Empty;

		[Column("title_sa")]
		public string TitleSa { get; set; } = string.Empty;

		[Column("description_en")]
		public string DescriptionEn { get; set; } = string.Empty;

		[Column("description_hi")]
		public string DescriptionHi { get; set; } = string.Empty;

		[Column("category_id")]
		public int CategoryId { get; set; }

		[Column("category_name")]
		public string CategoryName { get; set; } = string.Empty;

		[Column("class_id")]
		public int ClassId { get; set; }

		[Column("class_name")]
		public string ClassName { get; set; } = string.Empty;

		[Column("source_id")]
		public int SourceId { get; set; }

		[Column("search_weight")]
		public int SearchWeight { get; set; }

		[Column("meta_tags", SerializationMode = SerializationMode.Json)]
		public List<string> MetaTags { get; set; } = new();

		[Column("created_at")]
		public DateTimeOffset CreatedAt { get; set; }
	}

	public class DbScriptureDetail
	{
		[Column("scripture_id")]
		public int ScriptureId { get; set; }

		[Column("scripture_code")]
		public string ScriptureCode { get; set; } = string.Empty;

		[Column("scripture_title_en")]
		public string ScriptureTitleEn { get; set; } = string.Empty;

		[Column("scripture_title_hi")]
		public string ScriptureTitleHi { get; set; } = string.Empty;

		[Column("scripture_title_sa")]
		public string ScriptureTitleSa { get; set; } = string.Empty;

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

		[Column("hierarchy_title_en")]
		public string HierarchyTitleEn { get; set; } = string.Empty;

		[Column("hierarchy_title_hi")]
		public string HierarchyTitleHi { get; set; } = string.Empty;

		[Column("hierarchy_title_sa")]
		public string HierarchyTitleSa { get; set; } = string.Empty;

		[Column("hierarchy_description_en")]
		public string HierarchyDescriptionEn { get; set; } = string.Empty;

		[Column("hierarchy_description_hi")]
		public string HierarchyDescriptionHi { get; set; } = string.Empty;

		[Column("sequence_number")]
		public int SequenceNumber { get; set; }

		[Column("direct_verse_count")]
		public int DirectVerseCount { get; set; }

		[Column("recursive_verse_count")]
		public int RecursiveVerseCount { get; set; }
	}

	public class DbVerseDetail
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

		[Column("scripture_titles", SerializationMode = SerializationMode.Json)]
		public LocalizedTitles ScriptureTitles { get; set; } = new();

		[Column("scripture_description", SerializationMode = SerializationMode.Json)]
		public LocalizedDescription ScriptureDescription { get; set; } = new();

		[Column("scripture_meta_tags", SerializationMode = SerializationMode.Json)]
		public List<string> ScriptureMetaTags { get; set; } = new();
	}

	public class DbVerseIdForHierarchy
	{
		[Column("verse_id")]
		public int VerseId { get; set; }

		[Column("content_sanskrit")]
		public string ContentSanskrit { get; set; } = string.Empty;
	}

	public class DbVerseSearchResult
	{
		[Column("rank")]
		public float Rank { get; set; }

		[Column("verse_id")]
		public int VerseId { get; set; }

		[Column("verse_number")]
		public string VerseNumber { get; set; } = string.Empty;

		[Column("verse_type")]
		public VerseType VerseType { get; set; }

		[Column("content_sanskrit")]
		public string ContentSanskrit { get; set; } = string.Empty;

		[Column("translation_en")]
		public string TranslationEn { get; set; } = string.Empty;

		[Column("translation_hi")]
		public string TranslationHi { get; set; } = string.Empty;

		[Column("word_breakdown", SerializationMode = SerializationMode.Json)]
		public List<WordBreakdownItem> WordBreakdown { get; set; } = new();

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

		[Column("scripture_titles", SerializationMode = SerializationMode.Json)]
		public LocalizedTitles ScriptureTitles { get; set; } = new();

		[Column("scripture_description", SerializationMode = SerializationMode.Json)]
		public LocalizedDescription ScriptureDescription { get; set; } = new();

		[Column("scripture_meta_tags", SerializationMode = SerializationMode.Json)]
		public List<string> ScriptureMetaTags { get; set; } = new();
	}
}
