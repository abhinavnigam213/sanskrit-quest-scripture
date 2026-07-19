using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using SanskritQuest.Data.Providers;
using SanskritQuest.Common.Http;

namespace SanskritQuest.Services.AIService;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAIServices(this IServiceCollection services)
    {
        // Register Common HTTP Client services
        services.AddCommonHttp();

        var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        if (!string.IsNullOrEmpty(apiKey))
        {
            try
            {
                var clientOptions = new OpenAIClientOptions
                {
                    Endpoint = new Uri("https://generativelanguage.googleapis.com/v1beta/openai/")
                };

                // Use the common HttpClientFactory to resolve/create HttpClient for AI calls
                services.AddSingleton<IChatClient>(sp =>
                {
                    var httpFactory = sp.GetRequiredService<ICommonHttpClientFactory>();
                    var httpClient = httpFactory.CreateClient("AIServiceClient");

                    // Configure client options to use custom HttpClient via ClientModel transport
                    clientOptions.Transport = new HttpClientPipelineTransport(httpClient);

                    var openAIClient = new OpenAIClient(new ApiKeyCredential(apiKey), clientOptions);
                    return openAIClient.AsChatClient("gemini-1.5-flash");
                });

                Console.WriteLine("[Services.AIService] Registered Gemini ChatClient with Common HttpClient successfully.");
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
            sp.GetRequiredService<SanskritQuest.Data.Contracts.ILocalDataSetsProvider>()
        ));

        return services;
    }
}
