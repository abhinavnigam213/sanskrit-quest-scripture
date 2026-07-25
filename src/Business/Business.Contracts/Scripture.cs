using System.Collections.Generic;
using SanskritQuest.Common.Contracts;

namespace SanskritQuest.Business.Contracts
{
	public class Scripture
	{
		public int ScriptureId { get; set; }
		public string Code { get; set; } = string.Empty;
		public LocalizedTitles Titles { get; set; } = new();
		public LocalizedDescription? Author { get; set; }
		public LocalizedDescription Description { get; set; } = new();
		public int CategoryId { get; set; }
		public string CategoryName { get; set; } = string.Empty;
		public int ClassId { get; set; }
		public string ClassName { get; set; } = string.Empty;
		public int SourceId { get; set; }
		public int SearchWeight { get; set; }
		public List<string> MetaTags { get; set; } = new();
		public string EnumName { get; set; } = string.Empty;
	}
}
