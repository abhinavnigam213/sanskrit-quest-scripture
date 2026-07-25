using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SanskritQuest.Common.Contracts;

namespace SanskritQuest.Services.AIService.Contracts;

public interface ISanskritTranslationAIService
{
	Task<TranslationResult?> TranslateShlokaAsync(
		int index,
		string sanskritText,
		string? existingEnglish = null,
		CancellationToken cancellationToken = default,
		TranslationLanguage? target = null);

	Task<List<TranslationResult>?> TranslateShlokasBatchAsync(
		List<(int index, string sanskrit, string? english)> batch,
		CancellationToken cancellationToken = default,
		TranslationLanguage? target = null);
}
