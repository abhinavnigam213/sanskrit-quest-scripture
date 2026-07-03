using SanskritQuest.Data.Contracts;

namespace SanskritQuest.Business.Contracts
{

	public interface IScriptureProvider
	{
		List<Scripture> GetPopularScriptures();
	}
}
