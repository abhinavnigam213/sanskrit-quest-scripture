using SanskritQuest.Business.Contracts;
using SanskritQuest.Data.Contracts;

namespace SanskritQuest.Business.Providers;

public class ScriptureProvider : IScriptureProvider
{
	private readonly ILocalDataSetsProvider _localDataSetsProvider;

	public ScriptureProvider(ILocalDataSetsProvider localDataSetsProvider)
	{
		_localDataSetsProvider = localDataSetsProvider;
	}

	public List<Scripture> GetPopularScriptures()
	{
		return _localDataSetsProvider.PopularScriptures;
	}
}
