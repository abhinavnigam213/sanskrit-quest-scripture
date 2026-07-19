using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SanskritQuest.Common.Contracts;
using SanskritQuest.Data.Contracts;

namespace Data.TestConsole
{
	public static class ScripturesProviderTest
	{
		public static async Task<IEnumerable<DbScripture>> TestGetAllScripturesAsync(IScripturesDataProvider provider)
		{
			Console.WriteLine("\n[1] Testing GetAllScripturesAsync...");
			var scriptures = (await provider.GetAllScripturesAsync()).ToList();
			Console.WriteLine($"Found {scriptures.Count} scriptures.");
			foreach (var s in scriptures.Take(3))
			{
				Console.WriteLine($" - ID: {s.ScriptureId}, Code: {s.Code}, English Title: {s.Titles?.En}");
			}
			return scriptures;
		}

		public static async Task<IEnumerable<ScriptureDetail>> TestGetScriptureDetailsAsync(IScripturesDataProvider provider, int scriptureId, string code)
		{
			Console.WriteLine($"\n[2] Testing GetScriptureDetailsAsync for scripture ID {scriptureId} ({code})...");
			var details = (await provider.GetScriptureDetailsAsync(scriptureId)).ToList();
			Console.WriteLine($"Found {details.Count} hierarchy nodes.");
			foreach (var d in details.Take(5))
			{
				Console.WriteLine($" - Node ID: {d.HierarchyId}, Label: {d.LocalLabel}, Type: {d.NodeType}, English Title: {d.HierarchyTitles?.En}, Path: {d.Path}");
			}
			return details;
		}

		public static async Task<IEnumerable<VerseIdForHierarchy>> TestGetAllVersesIdForHierarchyAsync(IScripturesDataProvider provider, int hierarchyId, string localLabel)
		{
			Console.WriteLine($"\n[3] Testing GetAllVersesIdForHierarchyAsync for hierarchy ID {hierarchyId} ({localLabel})...");
			var verseIds = (await provider.GetAllVersesIdForHierarchyAsync(hierarchyId)).ToList();
			Console.WriteLine($"Found {verseIds.Count} verses in hierarchy.");
			foreach (var v in verseIds.Take(3))
			{
				Console.WriteLine($" - Verse ID: {v.VerseId}, Sanskrit snippet: {v.ContentSanskrit?.Substring(0, Math.Min(25, v.ContentSanskrit.Length))}...");
			}
			return verseIds;
		}

		public static async Task<VerseDetail?> TestGetVerseDetailsAsync(IScripturesDataProvider provider, int verseId)
		{
			Console.WriteLine($"\n[4] Testing GetVerseDetailsAsync for verse ID {verseId}...");
			var verseDetails = await provider.GetVerseDetailsAsync(verseId);
			if (verseDetails != null)
			{
				Console.WriteLine($" - Verse Number: {verseDetails.VerseNumber}");
				Console.WriteLine($" - Verse Type: {verseDetails.VerseType}");
				Console.WriteLine($" - Sanskrit Content: {verseDetails.ContentSanskrit}");
				Console.WriteLine($" - Translation (EN): {verseDetails.VerseData?.TranslationEn}");
				Console.WriteLine($" - Word Breakdown count: {verseDetails.WordByWordBreakdown?.Count ?? 0}");
			}
			else
			{
				Console.WriteLine(" - Verse details not found.");
			}
			return verseDetails;
		}

		public static async Task<IEnumerable<VerseSearchResult>> TestSearchVersesBySanskritFTSAsync(IScripturesDataProvider provider, string query)
		{
			Console.WriteLine($"\n[5] Testing SearchVersesBySanskritFTSAsync query: '{query}'...");
			var results = (await provider.SearchVersesBySanskritFTSAsync(query, 3)).ToList();
			Console.WriteLine($"Found {results.Count} search results.");
			foreach (var r in results)
			{
				Console.WriteLine($" - Rank: {r.Rank}, Verse ID: {r.VerseId}, Number: {r.VerseNumber}, Sanskrit: {r.ContentSanskrit}");
			}
			return results;
		}

		public static async Task<IEnumerable<VerseSearchResult>> TestSearchVersesByTranslationFTSAsync(IScripturesDataProvider provider, string query)
		{
			Console.WriteLine($"\n[6] Testing SearchVersesByTranslationFTSAsync query: '{query}'...");
			var results = (await provider.SearchVersesByTranslationFTSAsync(query, 3, TranslationLanguage.English)).ToList();
			Console.WriteLine($"Found {results.Count} search results.");
			foreach (var r in results)
			{
				Console.WriteLine($" - Rank: {r.Rank}, Verse ID: {r.VerseId}, Number: {r.VerseNumber}, Sanskrit: {r.ContentSanskrit}, English: {r.TranslationEn}");
			}
			return results;
		}
	}
}
