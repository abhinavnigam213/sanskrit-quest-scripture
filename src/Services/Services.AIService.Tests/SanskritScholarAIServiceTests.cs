using System.Text.Json;
using Microsoft.Extensions.AI;
using Moq;
using SanskritQuest.Business.Contracts;
using SanskritQuest.Data.Contracts;
using Xunit;

namespace SanskritQuest.Services.AIService.Tests;

public class SanskritScholarAIServiceTests
{
	private readonly Mock<IChatClient> _mockChatClient;
	private readonly Mock<ILocalDataSetsProvider> _mockLocalDataSetsProvider;
	private readonly SanskritScholarAIService _service;

	public SanskritScholarAIServiceTests()
	{
		_mockChatClient = new Mock<IChatClient>();
		_mockLocalDataSetsProvider = new Mock<ILocalDataSetsProvider>();

		// Set up default dictionaries to prevent null ref in fallback
		_mockLocalDataSetsProvider.Setup(p => p.SpecializedDictionary).Returns(new Dictionary<string, DictionaryEntry>());
		_mockLocalDataSetsProvider.Setup(p => p.CommonDictionary).Returns(new Dictionary<string, GenericWordDetails>());
		_mockLocalDataSetsProvider.Setup(p => p.PopularArchive).Returns(new Dictionary<string, ScriptureAnalyzeResponse>());

		_service = new SanskritScholarAIService(_mockChatClient.Object, _mockLocalDataSetsProvider.Object);
	}

	[Fact]
	public async Task TranslateTextAsync_Returns_TranslationResponse_On_Success()
	{
		// Arrange
		var mockResponse = new TranslationResponse(
			SourceLang: "sanskrit",
			TargetLang: "english",
			TranslatedText: "Translated text",
			Explanation: "Explanation details",
			WordBreakdown: new List<WordBreakdownItem>(),
			IsFallback: false
		);

		var json = JsonSerializer.Serialize(mockResponse);
		var chatResponse = new ChatCompletion(new ChatMessage(ChatRole.Assistant, json));

		_mockChatClient
			.Setup(c => c.CompleteAsync(It.IsAny<IList<ChatMessage>>(), It.IsAny<ChatOptions>(), default))
			.ReturnsAsync(chatResponse);

		// Act
		var result = await _service.TranslateTextAsync("Sanskrit Text", "sanskrit", "english", "context");

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Translated text", result.TranslatedText);
		Assert.False(result.IsFallback);
	}

	[Fact]
	public async Task TranslateTextAsync_Falls_Back_On_Exception()
	{
		// Arrange
		_mockChatClient
			.Setup(c => c.CompleteAsync(It.IsAny<IList<ChatMessage>>(), It.IsAny<ChatOptions>(), default))
			.ThrowsAsync(new Exception("API Error"));

		// Act
		var result = await _service.TranslateTextAsync("Sanskrit Text", "sanskrit", "english", "context");

		// Assert
		Assert.NotNull(result);
		Assert.True(result.IsFallback);
	}

	[Fact]
	public async Task TransliterateTextAsync_Returns_TransliterateResponse_On_Success()
	{
		// Arrange
		var mockResponse = new TransliterateResponse(
			SourceScript: "devanagari",
			TargetScript: "iast",
			TransliteratedText: "IAST Text"
		);

		var json = JsonSerializer.Serialize(mockResponse);
		var chatResponse = new ChatCompletion(new ChatMessage(ChatRole.Assistant, json));

		_mockChatClient
			.Setup(c => c.CompleteAsync(It.IsAny<IList<ChatMessage>>(), It.IsAny<ChatOptions>(), default))
			.ReturnsAsync(chatResponse);

		// Act
		var result = await _service.TransliterateTextAsync("Devanagari Text", "devanagari", "iast");

		// Assert
		Assert.NotNull(result);
		Assert.Equal("IAST Text", result.TransliteratedText);
	}

	[Fact]
	public async Task AnalyzeScriptureAsync_Returns_ScriptureAnalyzeResponse_On_Success()
	{
		// Arrange
		var mockResponse = new ScriptureAnalyzeResponse(
			Verse: "Verse Text",
			IdentifiedSource: "Source",
			TransliterationIAST: "IAST",
			TransliterationPhonetic: "Phonetic",
			TranslationEnglish: "English",
			TranslationHindi: "Hindi",
			SpiritualSignificance: "Significance",
			WordBreakdown: new List<WordBreakdownItem>(),
			PoeticMeter: "Meter",
			IsFallback: false
		);

		var json = JsonSerializer.Serialize(mockResponse);
		var chatResponse = new ChatCompletion(new ChatMessage(ChatRole.Assistant, json));

		_mockChatClient
			.Setup(c => c.CompleteAsync(It.IsAny<IList<ChatMessage>>(), It.IsAny<ChatOptions>(), default))
			.ReturnsAsync(chatResponse);

		// Act
		var result = await _service.AnalyzeScriptureAsync("Verse Text", "context");

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Verse Text", result.Verse);
		Assert.Equal("Source", result.IdentifiedSource);
		Assert.False(result.IsFallback);
	}
}
