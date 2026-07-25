using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using Moq;
using SanskritQuest.Common.Contracts;
using SanskritQuest.Services.AIService;
using SanskritQuest.Services.AIService.Contracts;
using Xunit;

namespace SanskritQuest.Services.AIService.Tests;

public class SanskritTranslationAIServiceTests
{
	private readonly Mock<IChatClient> _mockChatClient;
	private readonly SanskritTranslationAIService _service;

	public SanskritTranslationAIServiceTests()
	{
		_mockChatClient = new Mock<IChatClient>();
		_service = new SanskritTranslationAIService(_mockChatClient.Object);
	}

	[Fact]
	public async Task TranslateShlokaAsync_Returns_TranslationResult_On_Success()
	{
		// Arrange
		var json = @"[
			{
				""index"": 1,
				""translation_en"": ""English translation"",
				""translation_hi"": ""Hindi translation""
			}
		]";
		var chatResponse = new ChatCompletion(new ChatMessage(ChatRole.Assistant, json));

		_mockChatClient
			.Setup(c => c.CompleteAsync(It.IsAny<IList<ChatMessage>>(), It.IsAny<ChatOptions>(), default))
			.ReturnsAsync(chatResponse);

		// Act
		var result = await _service.TranslateShlokaAsync(1, "Sanskrit Text", null, default, TranslationLanguage.Both);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(1, result.Index);
		Assert.Equal("English translation", result.TranslationEn);
		Assert.Equal("Hindi translation", result.TranslationHi);
	}

	[Fact]
	public async Task TranslateShlokasBatchAsync_Returns_TranslationResults_On_Success()
	{
		// Arrange
		var json = @"[
			{
				""index"": 1,
				""translation_en"": ""English translation 1"",
				""translation_hi"": ""Hindi translation 1""
			},
			{
				""index"": 2,
				""translation_en"": ""English translation 2"",
				""translation_hi"": ""Hindi translation 2""
			}
		]";
		var chatResponse = new ChatCompletion(new ChatMessage(ChatRole.Assistant, json));

		_mockChatClient
			.Setup(c => c.CompleteAsync(It.IsAny<IList<ChatMessage>>(), It.IsAny<ChatOptions>(), default))
			.ReturnsAsync(chatResponse);

		var batchInput = new List<(int index, string sanskrit, string? english)>
		{
			(1, "Sanskrit 1", "English 1"),
			(2, "Sanskrit 2", null)
		};

		// Act
		var results = await _service.TranslateShlokasBatchAsync(batchInput, default, TranslationLanguage.Both);

		// Assert
		Assert.NotNull(results);
		Assert.Equal(2, results.Count);
		Assert.Equal("English translation 1", results[0].TranslationEn);
		Assert.Equal("Hindi translation 2", results[1].TranslationHi);
	}
}
