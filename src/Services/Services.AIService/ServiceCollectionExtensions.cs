using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SanskritQuest.Common.Configuration;
using SanskritQuest.Common.Http;
using SanskritQuest.Services.AIService.Contracts;
using SanskritQuest.Services.AIService.Providers;
using System;
using System.Linq;

namespace SanskritQuest.Services.AIService;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddAIServices(this IServiceCollection services)
	{
		return services.AddAIServicesInternal(null);
	}

	public static IServiceCollection AddAIServices(this IServiceCollection services, IConfiguration configuration)
	{
		return services.AddAIServicesInternal(configuration);
	}

	private static IServiceCollection AddAIServicesInternal(this IServiceCollection services, IConfiguration? configuration)
	{
		// 1. Ensure Common Http dependencies are loaded
		services.AddCommonHttp();

		// 2. Ensure AISettings is bound if not already present
		if (!services.Any(d => d.ServiceType == typeof(AISettings)))
		{
			services.AddSingleton<AISettings>(sp =>
			{
				var config = configuration ?? sp.GetRequiredService<IConfiguration>();
				var newSettings = new AISettings();
				config.GetSection("AISettings").Bind(newSettings);
				return newSettings;
			});
		}

		// 3. Register all IAIProviders
		services.AddSingleton<IAIProvider, GeminiAIProvider>();
		services.AddSingleton<IAIProvider, OpenAIAIProvider>();
		services.AddSingleton<IAIProvider, OllamaAIProvider>();

		// 4. Register dynamic IChatClient resolver based on AISettings
		services.AddSingleton<IChatClient>(sp =>
		{
			var aiSettings = sp.GetRequiredService<AISettings>();
			var activeProviderName = aiSettings.ActiveProvider ?? "Gemini";

			var providers = sp.GetServices<IAIProvider>();
			var activeProvider = providers.FirstOrDefault(p => p.ProviderName.Equals(activeProviderName, StringComparison.OrdinalIgnoreCase));

			if (activeProvider == null)
			{
				Console.WriteLine($"[Services.AIService] Warning: AI Provider '{activeProviderName}' is not registered or supported. Starting in offline fallback mode.");
				return null!;
			}

			try
			{
				return activeProvider.CreateChatClient();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Services.AIService] Warning: Failed to build AI ChatClient for '{activeProviderName}': {ex.Message}. Starting in offline fallback mode.");
				return null!;
			}
		});

		// 5. Register SanskritScholarAIService
		services.AddSingleton<ISanskritScholarAIService>(sp => new SanskritScholarAIService(
			sp.GetService<IChatClient>(),
			sp.GetRequiredService<SanskritQuest.Data.Contracts.ILocalDataSetsProvider>()
		));

		// 6. Register SanskritTranslationAIService
		services.AddSingleton<ISanskritTranslationAIService>(sp => new SanskritTranslationAIService(
			sp.GetService<IChatClient>()
		));

		return services;
	}
}
