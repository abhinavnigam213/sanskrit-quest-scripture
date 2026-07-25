using System;
using Microsoft.Extensions.AI;
using SanskritQuest.Common.Configuration;
using SanskritQuest.Common.Http;

namespace SanskritQuest.Services.AIService.Providers;

public class OllamaAIProvider : AIProviderBase
{
	public override string ProviderName => "Ollama";

	public OllamaAIProvider(AISettings aiSettings, ICommonHttpClientFactory httpClientFactory) 
		: base(aiSettings, httpClientFactory)
	{
	}

	public override IChatClient CreateChatClient()
	{
		if (!AiSettings.Providers.TryGetValue(ProviderName, out var settings))
		{
			throw new InvalidOperationException($"Configuration section for AI Provider '{ProviderName}' is missing.");
		}

		var modelId = string.IsNullOrEmpty(settings.ModelId) ? "llama3" : settings.ModelId;
		var endpoint = string.IsNullOrEmpty(settings.Endpoint) ? "http://localhost:11434/" : settings.Endpoint;

		// If Microsoft.Extensions.AI.Ollama package is added, we would initialize:
		// return new OllamaChatClient(new Uri(endpoint), modelId);
		throw new NotImplementedException("Ollama provider requires the Microsoft.Extensions.AI.Ollama package.");
	}
}
