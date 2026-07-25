using SanskritQuest.Common.Contracts;

namespace SanskritQuest.Services.AIService.Contracts;

public class TranslationResult : ILocalizedTranslation
{
	public int Index { get; set; }
	public string? TranslationEn { get; set; }
	public string? TranslationHi { get; set; }
}
