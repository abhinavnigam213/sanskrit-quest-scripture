using SanskritQuest.Common.Utilities;

namespace SanskritQuest.Data.Contracts
{
	public interface IDictionaryDataProvider
	{
		Task<IEnumerable<DictionaryRecord>> ExactSearchAsync(string query, SanskritScheme queryScheme, int topResult = 5, CancellationToken cancellationToken = default);

		Task<IEnumerable<DictionaryRecord>> FuzzySearchAsync(string query, SanskritScheme queryScheme, int topResult = 5, CancellationToken cancellationToken = default);

		Task<IEnumerable<DictionaryRecord>> KeywordSearchAsync(string query, SanskritScheme queryScheme, int topResult = 5, CancellationToken cancellationToken = default);

	}
}
