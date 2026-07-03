namespace SanskritQuest.Data.Contracts
{
	public interface ILocalDataSetsProvider
	{
		public List<Scripture> PopularScriptures { get; }

		public Dictionary<string, DictionaryEntry> SpecializedDictionary { get; }
		public Dictionary<string, GenericWordDetails> CommonDictionary { get; }
		public Dictionary<string, ScriptureAnalyzeResponse> PopularArchive { get; }

		public Dictionary<string, DictionaryEntry> VedasDict { get; }
		public Dictionary<string, DictionaryEntry> UpanishadsDict { get; }
		public Dictionary<string, DictionaryEntry> GitaDict { get; }
		public Dictionary<string, DictionaryEntry> RamayanaDict { get; }
		public Dictionary<string, DictionaryEntry> PuranasDict { get; }
	}
}
