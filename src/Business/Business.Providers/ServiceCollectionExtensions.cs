using Microsoft.Extensions.DependencyInjection;
using SanskritQuest.Business.Contracts;
using SanskritQuest.Data.Providers.Extensions;
using SanskritQuest.Services.AIService;

namespace SanskritQuest.Business.Providers;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddBusinessProviders(this IServiceCollection services)
	{
		services.AddDataProviders();
		services.AddAIServices();
		services.AddSingleton<IScriptureProvider, ScriptureProvider>();
		services.AddSingleton<IDictionaryProvider, DictionaryProvider>();
		services.AddSingleton<ILanguageProvider, LanguageProvider>();
		return services;
	}
}
