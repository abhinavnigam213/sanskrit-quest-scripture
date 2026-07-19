using System;
using System.Collections.Generic;
using Insight.Database;
using SanskritQuest.Common.Contracts;

namespace SanskritQuest.Data.Contracts
{
	public class DbScripture
	{
		[Column("scripture_id")]
		public int ScriptureId { get; set; }

		[Column("code")]
		public string Code { get; set; } = string.Empty;

		[Column("author", SerializationMode = SerializationMode.Json)]
		public LocalizedDescription? Author { get; set; }

		[Column("titles", SerializationMode = SerializationMode.Json)]
		public LocalizedTitles Titles { get; set; } = new();

		[Column("description", SerializationMode = SerializationMode.Json)]
		public LocalizedDescription Description { get; set; } = new();

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
}
