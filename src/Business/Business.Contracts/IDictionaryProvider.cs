namespace SanskritQuest.Business.Contracts
{
	public interface IDictionaryProvider
	{

		Dictionary<string, object> SearchDictionary(string word);

		Dictionary<string, object> GetAllDictionaryData();
	}
}
