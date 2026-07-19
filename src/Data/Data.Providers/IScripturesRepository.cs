using System.Collections.Generic;
using System.Threading.Tasks;
using Insight.Database;
using SanskritQuest.Data.Contracts;

namespace SanskritQuest.Data.Providers
{
	public interface IScripturesRepository
	{
		[Sql("SELECT * FROM scripture.GetAllScripture()")]
		Task<IEnumerable<DbScripture>> GetAllScripturesAsync();

		[Sql("SELECT * FROM scripture.GetScriptureDetails(@p_scripture_id)")]
		Task<IEnumerable<ScriptureDetail>> GetScriptureDetailsAsync(int p_scripture_id);

		[Sql("SELECT * FROM scripture.GetVersesDetails(@p_verse_id)")]
		Task<IEnumerable<VerseDetail>> GetVersesDetailsAsync(int p_verse_id);

		[Sql("SELECT * FROM scripture.GetAllVersesIdForHierarchy(@p_hierarchy_id)")]
		Task<IEnumerable<VerseIdForHierarchy>> GetAllVersesIdForHierarchyAsync(int p_hierarchy_id);

		[Sql("SELECT * FROM scripture.SearchVersesBySanskritFTS(@p_query_text, @p_max_rows, @p_scripture_id)")]
		Task<IEnumerable<VerseSearchResult>> SearchVersesBySanskritFTSAsync(string p_query_text, int p_max_rows, int? p_scripture_id);

		[Sql("SELECT * FROM scripture.SearchVersesByTranslationFTS(@p_query_text, @p_max_rows, @p_translation_lang, @p_scripture_id)")]
		Task<IEnumerable<VerseSearchResult>> SearchVersesByTranslationFTSAsync(string p_query_text, int p_max_rows, string p_translation_lang, int? p_scripture_id);
	}
}
