using System.Collections.Generic;

namespace SanskritQuest.Common.Configuration
{
	public class AISettings
	{
		public string ActiveProvider { get; set; } = "Gemini";
		public Dictionary<string, AIProviderSettings> Providers { get; set; } = new();
	}

	public class AIProviderSettings
	{
		public string ApiKey { get; set; } = string.Empty;
		public string ModelId { get; set; } = string.Empty;
		public string Endpoint { get; set; } = string.Empty;
	}
}
