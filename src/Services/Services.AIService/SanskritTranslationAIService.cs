using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;
using SanskritQuest.Common.Contracts;
using SanskritQuest.Services.AIService.Contracts;

namespace SanskritQuest.Services.AIService;

public class SanskritTranslationAIService : ISanskritTranslationAIService
{
	#region Prompts and Constants

	private const string SYSTEM_PROMPT_ENGLISH =
		"You are an expert scriptural Indologist. Translate Sanskrit shlokas accurately into clear, theological English.";

	private const string SYSTEM_PROMPT_HINDI =
		"You are an expert scriptural Indologist. Translate Sanskrit shlokas and their English translations into natural, scriptural Hindi.";

	private const string SYSTEM_PROMPT_BOTH =
		"You are an expert scriptural Indologist. Translate Sanskrit shlokas into clear theological English and natural scriptural Hindi.";

	private const string USER_PROMPT_TRANSLATION_HEADER =
		"Translate the following Sanskrit shlokas from Valmiki Ramayana into target language(s) as configured by schema.\n" +
		"Provide the response strictly as a JSON array matching the schema.\n" +
		"Items to translate:\n\n";

	private const string SCHEMA_INSTRUCTION_BOTH =
		"Each item in the output JSON array must be an object with the following fields:\n" +
		"- 'index': The integer index matching the input item.\n" +
		"- 'translation_en': The English translation/explanation.\n" +
		"- 'translation_hi': The Hindi translation/explanation.\n";

	private const string SCHEMA_INSTRUCTION_ENGLISH =
		"Each item in the output JSON array must be an object with the following fields:\n" +
		"- 'index': The integer index matching the input item.\n" +
		"- 'translation_en': The English translation/explanation.\n";

	private const string SCHEMA_INSTRUCTION_HINDI =
		"Each item in the output JSON array must be an object with the following fields:\n" +
		"- 'index': The integer index matching the input item.\n" +
		"- 'translation_hi': The Hindi translation/explanation.\n";

	#endregion

	private readonly IChatClient? _chatClient;

	public SanskritTranslationAIService(IChatClient? chatClient)
	{
		_chatClient = chatClient;
	}

	public async Task<TranslationResult?> TranslateShlokaAsync(
		int index,
		string sanskritText,
		string? existingEnglish = null,
		CancellationToken cancellationToken = default,
		TranslationLanguage? target = null)
	{
		var singleBatch = new List<(int index, string sanskrit, string? english)>
		{
			(index, sanskritText, existingEnglish)
		};

		var results = await TranslateShlokasBatchAsync(singleBatch, cancellationToken, target);
		return results != null && results.Count > 0 ? results[0] : null;
	}

	public async Task<List<TranslationResult>?> TranslateShlokasBatchAsync(
		List<(int index, string sanskrit, string? english)> batch,
		CancellationToken cancellationToken = default,
		TranslationLanguage? target = null)
	{
		if (batch == null || batch.Count == 0) return new List<TranslationResult>();
		if (_chatClient == null)
		{
			throw new InvalidOperationException("IChatClient is not configured.");
		}

		var activeTarget = target ?? TranslationLanguage.Both;

		// Build prompt listing the batch elements
		var promptBuilder = new StringBuilder();
		promptBuilder.AppendLine(USER_PROMPT_TRANSLATION_HEADER);

		string schemaInstruction = activeTarget switch
		{
			TranslationLanguage.English => SCHEMA_INSTRUCTION_ENGLISH,
			TranslationLanguage.Hindi => SCHEMA_INSTRUCTION_HINDI,
			_ => SCHEMA_INSTRUCTION_BOTH
		};
		promptBuilder.AppendLine(schemaInstruction);
		promptBuilder.AppendLine("Items:\n");

		foreach (var item in batch)
		{
			promptBuilder.AppendLine($"Index: {item.index}");
			promptBuilder.AppendLine($"Sanskrit Shloka: {item.sanskrit}");
			if (!string.IsNullOrEmpty(item.english))
			{
				promptBuilder.AppendLine($"Existing English Translation: {item.english}");
			}
			promptBuilder.AppendLine("---");
		}

		string systemInstruction = activeTarget switch
		{
			TranslationLanguage.English => SYSTEM_PROMPT_ENGLISH,
			TranslationLanguage.Hindi => SYSTEM_PROMPT_HINDI,
			_ => SYSTEM_PROMPT_BOTH
		};

		var options = new ChatOptions
		{
			ResponseFormat = ChatResponseFormat.Json,
			Temperature = 0.2f
		};

		var messages = new List<ChatMessage>
		{
			new ChatMessage(ChatRole.System, systemInstruction),
			new ChatMessage(ChatRole.User, promptBuilder.ToString())
		};

		var response = await _chatClient.CompleteAsync(messages, options, cancellationToken);
		string responseText = response.Message.Text ?? string.Empty;

		if (string.IsNullOrEmpty(responseText)) return null;

		var deserializeOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
		var apiResults = JsonSerializer.Deserialize<List<ApiTranslationItem>>(responseText, deserializeOptions);

		if (apiResults == null) return null;

		var results = new List<TranslationResult>();
		foreach (var item in apiResults)
		{
			results.Add(new TranslationResult
			{
				Index = item.Index,
				TranslationEn = item.TranslationEn,
				TranslationHi = item.TranslationHi
			});
		}

		return results;
	}

	private class ApiTranslationItem
	{
		[JsonPropertyName("index")]
		public int Index { get; set; }

		[JsonPropertyName("translation_en")]
		public string? TranslationEn { get; set; }

		[JsonPropertyName("translation_hi")]
		public string? TranslationHi { get; set; }
	}
}
