using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using Microsoft.Extensions.AI;
using OpenAI;
using SanskritQuest.Common.Configuration;
using SanskritQuest.Common.Http;

namespace SanskritQuest.Services.AIService.Providers;

public class OpenAIAIProvider : AIProviderBase
{
	public override string ProviderName => "OpenAI";

	public OpenAIAIProvider(AISettings aiSettings, ICommonHttpClientFactory httpClientFactory) 
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
			apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty;
		}

		var modelId = string.IsNullOrEmpty(settings.ModelId) ? "gpt-4o" : settings.ModelId;
		var endpoint = settings.Endpoint;

		if (string.IsNullOrEmpty(apiKey))
		{
			throw new InvalidOperationException("API Key for OpenAI provider is not configured.");
		}

		var clientOptions = new OpenAIClientOptions();
		if (!string.IsNullOrEmpty(endpoint))
		{
			clientOptions.Endpoint = new Uri(endpoint);
		}

		var httpClient = HttpClientFactory.CreateClient("OpenAIClient");
		clientOptions.Transport = new HttpClientPipelineTransport(httpClient);

		var openAIClient = new OpenAIClient(new ApiKeyCredential(apiKey), clientOptions);
		return openAIClient.AsChatClient(modelId);
	}
}
