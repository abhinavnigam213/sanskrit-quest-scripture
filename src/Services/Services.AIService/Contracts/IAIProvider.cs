using Microsoft.Extensions.AI;

namespace SanskritQuest.Services.AIService.Contracts;

public interface IAIProvider
{
	string ProviderName { get; }
	IChatClient CreateChatClient();
}
