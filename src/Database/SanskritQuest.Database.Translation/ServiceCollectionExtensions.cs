using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SanskritQuest.Database.Translation
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSanskritTranslationServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            // Load Options from either root configuration or a nested "Translation" section
            var options = new TranslationOptions
            {
                GeminiApiKey = configuration["GeminiApiKey"] ?? "",
                ModelName = configuration["ModelName"] ?? "gemini-1.5-flash",
                ApiEndpointUrl = configuration["ApiEndpointUrl"] ?? "https://generativelanguage.googleapis.com/v1beta/models"
            };

            if (Enum.TryParse<TranslationTarget>(configuration["Target"], out var rootTarget))
            {
                options.Target = rootTarget;
            }

            if (float.TryParse(configuration["Temperature"], out var rootTemp))
            {
                options.Temperature = rootTemp;
            }

            var section = configuration.GetSection("Translation");
            if (section.Exists())
            {
                if (string.IsNullOrEmpty(options.GeminiApiKey))
                {
                    options.GeminiApiKey = section["GeminiApiKey"] ?? "";
                }
                if (!string.IsNullOrEmpty(section["ModelName"]))
                {
                    options.ModelName = section["ModelName"]!;
                }
                if (!string.IsNullOrEmpty(section["ApiEndpointUrl"]))
                {
                    options.ApiEndpointUrl = section["ApiEndpointUrl"]!;
                }
                if (Enum.TryParse<TranslationTarget>(section["Target"], out var secTarget))
                {
                    options.Target = secTarget;
                }
                if (float.TryParse(section["Temperature"], out var secTemp))
                {
                    options.Temperature = secTemp;
                }
            }

            services.AddSingleton(options);

            // Register SanskritTranslationService as a Typed HTTP Client
            services.AddHttpClient<ISanskritTranslationService, SanskritTranslationService>();

            return services;
        }
    }
}
