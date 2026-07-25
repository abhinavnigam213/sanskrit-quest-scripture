using System;
using Microsoft.Extensions.AI;
using SanskritQuest.Common.Configuration;
using SanskritQuest.Common.Http;
using SanskritQuest.Services.AIService.Contracts;

namespace SanskritQuest.Services.AIService.Providers;

public abstract class AIProviderBase : IAIProvider
{
	public abstract string ProviderName { get; }
	protected readonly AISettings AiSettings;
	protected readonly ICommonHttpClientFactory HttpClientFactory;

	protected AIProviderBase(AISettings aiSettings, ICommonHttpClientFactory httpClientFactory)
	{
		AiSettings = aiSettings ?? throw new ArgumentNullException(nameof(aiSettings));
		HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
	}

	public abstract IChatClient CreateChatClient();
}
