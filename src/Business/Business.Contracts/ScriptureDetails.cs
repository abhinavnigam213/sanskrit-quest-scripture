using System.Collections.Generic;
using SanskritQuest.Common.Contracts;

namespace SanskritQuest.Business.Contracts
{
	public class ScriptureDetails
	{
		public int ScriptureId { get; set; }
		public string Code { get; set; } = string.Empty;
		public LocalizedTitles Titles { get; set; } = new();
		public LocalizedDescription? Author { get; set; }
		public LocalizedDescription Description { get; set; } = new();
		public string EnumName { get; set; } = string.Empty;
		public List<ScriptureHierarchyNode> Hierarchy { get; set; } = new();
	}
}
