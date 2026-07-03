using SanskritQuest.Data.Contracts;

namespace SanskritQuest.Business.Contracts
{
	public interface ILanguageProvider
	{
		Task<TranslationResponse> TranslateTextAsync(TranslationRequest request);

		Task<TransliterateResponse> TransliterateTextAsync(TransliterateRequest request);

		Task<ScriptureAnalyzeResponse> AnalyzeScriptureAsync(AnalyzeRequest request);
	}
}
