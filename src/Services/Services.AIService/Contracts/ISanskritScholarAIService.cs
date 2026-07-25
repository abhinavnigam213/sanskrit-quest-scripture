using System.Threading.Tasks;
using SanskritQuest.Business.Contracts;
using SanskritQuest.Data.Contracts;

namespace SanskritQuest.Services.AIService.Contracts;

public interface ISanskritScholarAIService
{
	Task<TranslationResponse> TranslateTextAsync(
		string text,
		string sourceLang,
		string targetLang,
		string? scriptureContext);

	Task<TransliterateResponse> TransliterateTextAsync(
		string text,
		string sourceScript,
		string targetScript);

	Task<ScriptureAnalyzeResponse> AnalyzeScriptureAsync(
		string text,
		string? sourceContext);
}
