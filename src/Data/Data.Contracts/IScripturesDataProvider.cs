using SanskritQuest.Common.Contracts;

namespace SanskritQuest.Data.Contracts
{
	public interface IScripturesDataProvider
	{
		Task<IEnumerable<DbScripture>> GetAllScripturesAsync(CancellationToken cancellationToken = default);

		Task<IEnumerable<ScriptureDetail>> GetScriptureDetailsAsync(int scriptureId, CancellationToken cancellationToken = default);

		Task<VerseDetail?> GetVerseDetailsAsync(int verseId, CancellationToken cancellationToken = default);

		Task<IEnumerable<VerseIdForHierarchy>> GetAllVersesIdForHierarchyAsync(int hierarchyId, CancellationToken cancellationToken = default);

		Task<IEnumerable<VerseSearchResult>> SearchVersesBySanskritFTSAsync(string queryText, int maxRows = 10, int? scriptureId = null, CancellationToken cancellationToken = default);

		Task<IEnumerable<VerseSearchResult>> SearchVersesByTranslationFTSAsync(string queryText, int maxRows = 10, TranslationLanguage translationLang = TranslationLanguage.Both, int? scriptureId = null, CancellationToken cancellationToken = default);
	}
}
