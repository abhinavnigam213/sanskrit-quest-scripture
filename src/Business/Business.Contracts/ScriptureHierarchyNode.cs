using System.Collections.Generic;
using SanskritQuest.Common.Contracts;

namespace SanskritQuest.Business.Contracts
{
	public class ScriptureHierarchyNode
	{
		public int HierarchyId { get; set; }
		public int? ParentId { get; set; }
		public string LocalLabel { get; set; } = string.Empty;
		public string Path { get; set; } = string.Empty;
		public HierarchyNodeType NodeType { get; set; }
		public LocalizedTitles Titles { get; set; } = new();
		public LocalizedDescription Description { get; set; } = new();
		public int SequenceNumber { get; set; }
		public int DirectVerseCount { get; set; }
		public int RecursiveVerseCount { get; set; }
		public string FullHierarchyLabel { get; set; } = string.Empty;
		public List<ScriptureHierarchyNode> Children { get; set; } = new();
	}
}
