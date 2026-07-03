using SanskritQuest.Business.Contracts;
using SanskritQuest.Data.Contracts;

namespace SanskritQuest.Business.Providers;

public class DictionaryProvider(ILocalDataSetsProvider localDataSetsProvider) : IDictionaryProvider
{
	private readonly ILocalDataSetsProvider _localDataSetsProvider = localDataSetsProvider;

	public Dictionary<string, object> GetAllDictionaryData()
	{
		return new Dictionary<string, object>
		{
			["Vedas"] = _localDataSetsProvider.VedasDict,
			["Upanishads"] = _localDataSetsProvider.UpanishadsDict,
			["Gita"] = _localDataSetsProvider.GitaDict,
			["Ramayana"] = _localDataSetsProvider.RamayanaDict,
			["Puranas"] = _localDataSetsProvider.PuranasDict,
			["all"] = _localDataSetsProvider.SpecializedDictionary
		};
	}

	public Dictionary<string, object> SearchDictionary(string word)
	{
		string cleanWord = word.Trim().ToLower();

		// Exact match
		var exactMatch = _localDataSetsProvider.SpecializedDictionary.FirstOrDefault(
			kv => kv.Key.Equals(cleanWord, StringComparison.OrdinalIgnoreCase)
		);

		if (exactMatch.Key != null)
		{
			return new Dictionary<string, object>
			{
				["word"] = exactMatch.Key,
				["found"] = true,
				["entry"] = exactMatch.Value
			};
		}

		// Partial match
		var partialMatches = _localDataSetsProvider.SpecializedDictionary.Where(kv =>
			kv.Key.Contains(cleanWord, StringComparison.OrdinalIgnoreCase) ||
			kv.Value.Eng.Contains(cleanWord, StringComparison.OrdinalIgnoreCase) ||
			kv.Value.Hin.Contains(cleanWord, StringComparison.OrdinalIgnoreCase)
		).ToDictionary(kv => kv.Key, kv => kv.Value);

		if (partialMatches.Count > 0)
		{
			return new Dictionary<string, object>
			{
				["word"] = word,
				["found"] = true,
				["message"] = $"Specific term not found, but found {partialMatches.Count} matching entry/entries.",
				["matches"] = partialMatches
			};
		}

		return new Dictionary<string, object>
		{
			["word"] = word,
			["found"] = false,
			["message"] = $"Word \"{word}\" not found in our specialized scriptures dictionary. Try querying \"all\" to retrieve all entries.",
			["availableCategories"] = new[] { "Vedas", "Upanishads", "Gita", "Ramayana", "Puranas" }
		};
	}

	//public Dictionary<string, object> GetDictionaryData(string? word)
	//{
	//	if (!string.IsNullOrEmpty(word))
	//	{
	//		string cleanWord = word.Trim().ToLower();

	//		if (cleanWord == "all" || cleanWord == "al")
	//		{
	//			return new Dictionary<string, object>
	//			{
	//				["Vedas"] = _localDataSetsProvider.VedasDict,
	//				["Upanishads"] = _localDataSetsProvider.UpanishadsDict,
	//				["Gita"] = _localDataSetsProvider.GitaDict,
	//				["Ramayana"] = _localDataSetsProvider.RamayanaDict,
	//				["Puranas"] = _localDataSetsProvider.PuranasDict,
	//				["all"] = _localDataSetsProvider.SpecializedDictionary
	//			};
	//		}

	//		// Exact match
	//		var exactMatch = _localDataSetsProvider.SpecializedDictionary.FirstOrDefault(
	//			kv => kv.Key.Equals(cleanWord, StringComparison.OrdinalIgnoreCase)
	//		);

	//		if (exactMatch.Key != null)
	//		{
	//			return new Dictionary<string, object>
	//			{
	//				["word"] = exactMatch.Key,
	//				["found"] = true,
	//				["entry"] = exactMatch.Value
	//			};
	//		}

	//		// Partial match
	//		var partialMatches = _localDataSetsProvider.SpecializedDictionary.Where(kv =>
	//			kv.Key.Contains(cleanWord, StringComparison.OrdinalIgnoreCase) ||
	//			kv.Value.Eng.Contains(cleanWord, StringComparison.OrdinalIgnoreCase) ||
	//			kv.Value.Hin.Contains(cleanWord, StringComparison.OrdinalIgnoreCase)
	//		).ToDictionary(kv => kv.Key, kv => kv.Value);

	//		if (partialMatches.Count > 0)
	//		{
	//			return new Dictionary<string, object>
	//			{
	//				["word"] = word,
	//				["found"] = true,
	//				["message"] = $"Specific term not found, but found {partialMatches.Count} matching entry/entries.",
	//				["matches"] = partialMatches
	//			};
	//		}

	//		return new Dictionary<string, object>
	//		{
	//			["word"] = word,
	//			["found"] = false,
	//			["message"] = $"Word \"{word}\" not found in our specialized scriptures dictionary. Try querying \"all\" to retrieve all entries.",
	//			["availableCategories"] = new[] { "Vedas", "Upanishads", "Gita", "Ramayana", "Puranas" }
	//		};
	//	}

	//	return new Dictionary<string, object>
	//	{
	//		["Vedas"] = _localDataSetsProvider.VedasDict,
	//		["Upanishads"] = _localDataSetsProvider.UpanishadsDict,
	//		["Gita"] = _localDataSetsProvider.GitaDict,
	//		["Ramayana"] = _localDataSetsProvider.RamayanaDict,
	//		["Puranas"] = _localDataSetsProvider.PuranasDict,
	//		["all"] = _localDataSetsProvider.SpecializedDictionary
	//	};
	//}


}
