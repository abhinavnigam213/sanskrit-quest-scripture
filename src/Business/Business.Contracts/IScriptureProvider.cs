using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SanskritQuest.Business.Contracts
{
	public interface IScriptureProvider
	{
		List<Scripture> GetPopularScriptures();

		Task<IEnumerable<Scripture>> GetAllScripturesAsync(CancellationToken cancellationToken = default);

		Task<ScriptureDetails?> GetScriptureDetailsAsync(int scriptureId, CancellationToken cancellationToken = default);

		Task<VerseDetails?> GetVerseDetailsAsync(int verseId, CancellationToken cancellationToken = default);

		Task<VerseDetails?> GetVerseDetailsByHierarchyAsync(string scriptureCode, int[] levelNumbers, CancellationToken cancellationToken = default);
	}
}
