using Microsoft.Extensions.DependencyInjection;
using SanskritQuest.Business.Contracts;

namespace SanskritQuest.Business.Providers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessProviders(this IServiceCollection services)
    {
        services.AddSingleton<IScriptureService, ScriptureService>();
        services.AddSingleton<IDictionaryService, DictionaryService>();
        services.AddSingleton<ITranslationService, TranslationService>();
        services.AddSingleton<ITransliterateService, TransliterateService>();
        services.AddSingleton<IAnalyzeService, AnalyzeService>();
        return services;
    }
}
