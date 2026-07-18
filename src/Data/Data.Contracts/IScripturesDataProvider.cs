using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SanskritQuest.Data.Contracts
{
	public interface IScripturesDataProvider
	{
		Task<IEnumerable<DbScripture>> GetAllScripturesAsync(CancellationToken cancellationToken = default);

		Task<IEnumerable<DbScriptureDetail>> GetScriptureDetailsAsync(int scriptureId, CancellationToken cancellationToken = default);

		Task<DbVerseDetail?> GetVerseDetailsAsync(int verseId, CancellationToken cancellationToken = default);

		Task<IEnumerable<DbVerseIdForHierarchy>> GetAllVersesIdForHierarchyAsync(int hierarchyId, CancellationToken cancellationToken = default);

		Task<IEnumerable<DbVerseSearchResult>> SearchVersesBySanskritFTSAsync(string queryText, int maxRows = 10, int? scriptureId = null, CancellationToken cancellationToken = default);

		Task<IEnumerable<DbVerseSearchResult>> SearchVersesByTranslationFTSAsync(string queryText, int maxRows = 10, TranslationLanguage translationLang = TranslationLanguage.Both, int? scriptureId = null, CancellationToken cancellationToken = default);
	}
}
