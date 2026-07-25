using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using Microsoft.Extensions.AI;
using OpenAI;
using SanskritQuest.Common.Configuration;
using SanskritQuest.Common.Http;

namespace SanskritQuest.Services.AIService.Providers;

public class GeminiAIProvider : AIProviderBase
{
	public override string ProviderName => "Gemini";

	public GeminiAIProvider(AISettings aiSettings, ICommonHttpClientFactory httpClientFactory) 
		: base(aiSettings, httpClientFactory)
	{
	}

	public override IChatClient CreateChatClient()
	{
		if (!AiSettings.Providers.TryGetValue(ProviderName, out var settings))
		{
			throw new InvalidOperationException($"Configuration section for AI Provider '{ProviderName}' is missing.");
		}

		var apiKey = settings.ApiKey;
		if (string.IsNullOrEmpty(apiKey))
		{
			apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? string.Empty;
		}

		var modelId = string.IsNullOrEmpty(settings.ModelId) ? "gemini-1.5-flash" : settings.ModelId;
		var endpoint = string.IsNullOrEmpty(settings.Endpoint) ? "https://generativelanguage.googleapis.com/v1beta/openai/" : settings.Endpoint;

		if (string.IsNullOrEmpty(apiKey))
		{
			throw new InvalidOperationException("API Key for Gemini provider is not configured.");
		}

		var clientOptions = new OpenAIClientOptions
		{
			Endpoint = new Uri(endpoint)
		};

		var httpClient = HttpClientFactory.CreateClient("GeminiClient");
		clientOptions.Transport = new HttpClientPipelineTransport(httpClient);

		var openAIClient = new OpenAIClient(new ApiKeyCredential(apiKey), clientOptions);
		return openAIClient.AsChatClient(modelId);
	}
}
