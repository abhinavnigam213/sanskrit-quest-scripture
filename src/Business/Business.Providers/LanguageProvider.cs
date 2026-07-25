using System.Threading.Tasks;
using SanskritQuest.Business.Contracts;
using SanskritQuest.Data.Contracts;
using SanskritQuest.Services.AIService.Contracts;

namespace SanskritQuest.Business.Providers;

public class LanguageProvider : ILanguageProvider
{
	private readonly ISanskritScholarAIService _aiService;

	public LanguageProvider(ISanskritScholarAIService aiService)
	{
		_aiService = aiService;
	}

	public Task<ScriptureAnalyzeResponse> AnalyzeScriptureAsync(AnalyzeRequest request)
	{
		return _aiService.AnalyzeScriptureAsync(
			request.Text,
			request.SourceContext
		);
	}

	public Task<TranslationResponse> TranslateTextAsync(TranslationRequest request)
	{
		return _aiService.TranslateTextAsync(
			request.Text,
			request.SourceLang ?? "auto",
			request.TargetLang,
			request.ScriptureContext
		);
	}

	public Task<TransliterateResponse> TransliterateTextAsync(TransliterateRequest request)
	{
		return _aiService.TransliterateTextAsync(
			request.Text,
			request.SourceScript,
			request.TargetScript
		);
	}
}
