using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using System;
using System.ClientModel;
using SanskritQuest.Data.Providers;

namespace SanskritQuest.Services.AIService;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAIServices(this IServiceCollection services)
    {
        var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        if (!string.IsNullOrEmpty(apiKey))
        {
            try
            {
                var clientOptions = new OpenAIClientOptions
                {
                    Endpoint = new Uri("https://generativelanguage.googleapis.com/v1beta/openai/")
                };
                var openAIClient = new OpenAIClient(new ApiKeyCredential(apiKey), clientOptions);
                IChatClient chatClient = openAIClient.AsChatClient("gemini-1.5-flash");
                services.AddSingleton<IChatClient>(chatClient);
                Console.WriteLine("[Services.AIService] Registered Gemini ChatClient successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Services.AIService] Warning: Failed to build AI ChatClient: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("[Services.AIService] Warning: GEMINI_API_KEY is not defined. Server starting in OFFLINE fallback mode.");
        }

        // Register AIService using factory method to optionally resolve IChatClient (which can be null in offline mode)
        services.AddSingleton<AIService>(sp => new AIService(
            sp.GetService<IChatClient>(),
            sp.GetRequiredService<DataService>()
        ));

        return services;
    }
}
