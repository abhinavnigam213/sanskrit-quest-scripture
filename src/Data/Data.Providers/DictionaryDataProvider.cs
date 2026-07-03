using Insight.Database;
using SanskritQuest.Common.Utilities;
using SanskritQuest.Data.Contracts;

namespace SanskritQuest.Data.Providers
{
	public class DictionaryDataProvider : IDictionaryDataProvider
	{
		private readonly IDbConnectionFactory _connFactory;

		public DictionaryDataProvider(IDbConnectionFactory connFactory)
		{
			_connFactory = connFactory;
		}

		public async Task<IEnumerable<DictionaryRecord>> ExactSearchAsync(string query, SanskritScheme queryScheme, int topResult = 5, CancellationToken cancellationToken = default)
		{
			using (var conn = _connFactory.GetPocDbConnection())
			{
				string sql;
				if (queryScheme == SanskritScheme.Devanagari)
				{
					sql = @"
					SELECT * FROM dictionary_entries 
					WHERE devanagari = @Query OR devanagari LIKE @Query || '%'
					ORDER BY length(devanagari) ASC
					LIMIT @Limit;";
				}
				else if (queryScheme == SanskritScheme.SLP1)
				{
					sql = @"
					SELECT * FROM dictionary_entries 
					WHERE slp1 = @Query OR slp1 LIKE @Query || '%'
					ORDER BY length(slp1) ASC
					LIMIT @Limit;";
				}
				else
				{
					sql = @"
					SELECT * FROM dictionary_entries 
					WHERE iast = @Query OR iast LIKE @Query || '%'
					ORDER BY length(iast) ASC
					LIMIT @Limit;";
				}

				return await conn.QuerySqlAsync<DictionaryRecord>(sql, new { Query = query, Limit = topResult });
			}
		}

		public Task<IEnumerable<DictionaryRecord>> FuzzySearchAsync(string query, SanskritScheme queryScheme, int topResult = 5, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<DictionaryRecord>> KeywordSearchAsync(string query, SanskritScheme queryScheme, int topResult = 5, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
	}
}
