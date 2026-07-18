using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database;
using SanskritQuest.Data.Contracts;

namespace SanskritQuest.Data.Providers
{
	public class ScripturesDataProvider : IScripturesDataProvider
	{
		private readonly IDbConnectionFactory _connFactory;
		private readonly IScripturesRepository? _testRepository;

		public ScripturesDataProvider(IDbConnectionFactory connFactory)
		{
			_connFactory = connFactory;
		}

		// Constructor used for unit tests
		public ScripturesDataProvider(IDbConnectionFactory connFactory, IScripturesRepository repository)
			: this(connFactory)
		{
			_testRepository = repository;
		}

		private IScripturesRepository GetRepository(System.Data.IDbConnection conn)
		{
			return _testRepository ?? conn.As<IScripturesRepository>();
		}

		public async Task<IEnumerable<DbScripture>> GetAllScripturesAsync(CancellationToken cancellationToken = default)
		{
			using (var conn = _connFactory.GetDefaultDbConnection())
			{
				return await GetRepository(conn).GetAllScripturesAsync();
			}
		}

		public async Task<IEnumerable<DbScriptureDetail>> GetScriptureDetailsAsync(int scriptureId, CancellationToken cancellationToken = default)
		{
			using (var conn = _connFactory.GetDefaultDbConnection())
			{
				return await GetRepository(conn).GetScriptureDetailsAsync(scriptureId);
			}
		}

		public async Task<DbVerseDetail?> GetVerseDetailsAsync(int verseId, CancellationToken cancellationToken = default)
		{
			using (var conn = _connFactory.GetDefaultDbConnection())
			{
				var results = await GetRepository(conn).GetVersesDetailsAsync(verseId);
				return results.FirstOrDefault();
			}
		}

		public async Task<IEnumerable<DbVerseIdForHierarchy>> GetAllVersesIdForHierarchyAsync(int hierarchyId, CancellationToken cancellationToken = default)
		{
			using (var conn = _connFactory.GetDefaultDbConnection())
			{
				return await GetRepository(conn).GetAllVersesIdForHierarchyAsync(hierarchyId);
			}
		}

		public async Task<IEnumerable<DbVerseSearchResult>> SearchVersesBySanskritFTSAsync(string queryText, int maxRows = 10, int? scriptureId = null, CancellationToken cancellationToken = default)
		{
			using (var conn = _connFactory.GetDefaultDbConnection())
			{
				return await GetRepository(conn).SearchVersesBySanskritFTSAsync(queryText, maxRows, scriptureId);
			}
		}

		public async Task<IEnumerable<DbVerseSearchResult>> SearchVersesByTranslationFTSAsync(string queryText, int maxRows = 10, TranslationLanguage translationLang = TranslationLanguage.Both, int? scriptureId = null, CancellationToken cancellationToken = default)
		{
			string langStr = translationLang.ToString().ToLowerInvariant();

			using (var conn = _connFactory.GetDefaultDbConnection())
			{
				return await GetRepository(conn).SearchVersesByTranslationFTSAsync(queryText, maxRows, langStr, scriptureId);
			}
		}
	}
}
