using Microsoft.Extensions.DependencyInjection;
using SanskritQuest.Business.Contracts;
using SanskritQuest.Data.Providers;
using SanskritQuest.Services.AIService;

namespace SanskritQuest.Business.Providers;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddBusinessProviders(this IServiceCollection services)
	{
		services.AddDataProviders();
		services.AddAIServices();
		services.AddSingleton<IScriptureService, ScriptureService>();
		services.AddSingleton<IDictionaryService, DictionaryService>();
		services.AddSingleton<ITranslationService, TranslationService>();
		services.AddSingleton<ITransliterateService, TransliterateService>();
		services.AddSingleton<IAnalyzeService, AnalyzeService>();
		return services;
	}
}
