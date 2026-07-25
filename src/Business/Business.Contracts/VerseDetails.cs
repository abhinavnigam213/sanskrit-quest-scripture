using System.Collections.Generic;

namespace SanskritQuest.Business.Contracts
{
	public class VerseDetails
	{
		public string ScriptureName { get; set; } = string.Empty;
		public List<HierarchyLevel> Hierarchy { get; set; } = new();
		public string SanskritShloka { get; set; } = string.Empty;
		public string Transliteration { get; set; } = string.Empty;
		public List<WordBreakdown> WordByWordBreakdown { get; set; } = new();
		public Translation Translation { get; set; } = new();
		public List<Commentary> Commentaries { get; set; } = new();
	}

	public class HierarchyLevel
	{
		public string LevelType { get; set; } = string.Empty;
		public int LevelNumber { get; set; }
		public string? LevelNameSanskrit { get; set; }
	}

	public class WordBreakdown
	{
		public string SanskritWord { get; set; } = string.Empty;
		public string Transliteration { get; set; } = string.Empty;
		public string? TranslationEn { get; set; }
		public string? TranslationHi { get; set; }
	}

	public class Translation
	{
		public string? TranslationEn { get; set; }
		public string? TranslationHi { get; set; }
	}

	public class Commentary
	{
		public string Author { get; set; } = string.Empty;
		public string EnglishCommentary { get; set; } = string.Empty;
		public string HindiCommentary { get; set; } = string.Empty;
	}
}
