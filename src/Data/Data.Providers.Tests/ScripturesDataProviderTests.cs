using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SanskritQuest.Data.Contracts;
using SanskritQuest.Data.Providers;
using Xunit;

namespace SanskritQuest.Data.Providers.Tests
{
	public class ScripturesDataProviderTests
	{
		private readonly Mock<IDbConnectionFactory> _mockConnFactory;
		private readonly Mock<IDbConnection> _mockConnection;
		private readonly Mock<IScripturesRepository> _mockRepo;
		private readonly ScripturesDataProvider _provider;

		public ScripturesDataProviderTests()
		{
			_mockConnFactory = new Mock<IDbConnectionFactory>();
			_mockConnection = new Mock<IDbConnection>();
			_mockRepo = new Mock<IScripturesRepository>();

			_mockConnFactory.Setup(f => f.GetDefaultDbConnection()).Returns(_mockConnection.Object);
			_provider = new ScripturesDataProvider(_mockConnFactory.Object, _mockRepo.Object);
		}

		[Fact]
		public async Task GetAllScripturesAsync_Should_Call_Repository_Method()
		{
			// Arrange
			var expected = new List<DbScripture>
			{
				new DbScripture { ScriptureId = 1, Code = "Gita" }
			};
			_mockRepo.Setup(r => r.GetAllScripturesAsync()).ReturnsAsync(expected);

			// Act
			var result = await _provider.GetAllScripturesAsync();

			// Assert
			Assert.NotNull(result);
			Assert.Single(result);
			Assert.Equal("Gita", result.First().Code);
			_mockRepo.Verify(r => r.GetAllScripturesAsync(), Times.Once);
		}

		[Fact]
		public async Task GetScriptureDetailsAsync_Should_Call_Repository_Method_With_Parameters()
		{
			// Arrange
			int scriptureId = 5;
			var expected = new List<DbScriptureDetail>
			{
				new DbScriptureDetail { ScriptureId = 5, ScriptureCode = "Ramayana" }
			};
			_mockRepo.Setup(r => r.GetScriptureDetailsAsync(scriptureId)).ReturnsAsync(expected);

			// Act
			var result = await _provider.GetScriptureDetailsAsync(scriptureId);

			// Assert
			Assert.NotNull(result);
			Assert.Single(result);
			Assert.Equal("Ramayana", result.First().ScriptureCode);
			_mockRepo.Verify(r => r.GetScriptureDetailsAsync(scriptureId), Times.Once);
		}

		[Fact]
		public async Task GetVerseDetailsAsync_Should_Return_First_Element_If_Exists()
		{
			// Arrange
			int verseId = 42;
			var expected = new List<DbVerseDetail>
			{
				new DbVerseDetail { VerseId = verseId, VerseNumber = "1.1" }
			};
			_mockRepo.Setup(r => r.GetVersesDetailsAsync(verseId)).ReturnsAsync(expected);

			// Act
			var result = await _provider.GetVerseDetailsAsync(verseId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(verseId, result!.VerseId);
			Assert.Equal("1.1", result.VerseNumber);
			_mockRepo.Verify(r => r.GetVersesDetailsAsync(verseId), Times.Once);
		}

		[Fact]
		public async Task GetVerseDetailsAsync_Should_Return_Null_If_Empty()
		{
			// Arrange
			int verseId = 999;
			var expected = new List<DbVerseDetail>();
			_mockRepo.Setup(r => r.GetVersesDetailsAsync(verseId)).ReturnsAsync(expected);

			// Act
			var result = await _provider.GetVerseDetailsAsync(verseId);

			// Assert
			Assert.Null(result);
			_mockRepo.Verify(r => r.GetVersesDetailsAsync(verseId), Times.Once);
		}

		[Fact]
		public async Task GetAllVersesIdForHierarchyAsync_Should_Call_Repository_Method()
		{
			// Arrange
			int hierarchyId = 10;
			var expected = new List<DbVerseIdForHierarchy>
			{
				new DbVerseIdForHierarchy { VerseId = 101, ContentSanskrit = "राम" }
			};
			_mockRepo.Setup(r => r.GetAllVersesIdForHierarchyAsync(hierarchyId)).ReturnsAsync(expected);

			// Act
			var result = await _provider.GetAllVersesIdForHierarchyAsync(hierarchyId);

			// Assert
			Assert.NotNull(result);
			Assert.Single(result);
			Assert.Equal(101, result.First().VerseId);
			_mockRepo.Verify(r => r.GetAllVersesIdForHierarchyAsync(hierarchyId), Times.Once);
		}

		[Fact]
		public async Task SearchVersesBySanskritFTSAsync_Should_Call_Repository_Method_With_Parameters()
		{
			// Arrange
			string query = "धर्म";
			int maxRows = 10;
			int? scriptureId = 1;
			var expected = new List<DbVerseSearchResult>
			{
				new DbVerseSearchResult { VerseId = 201, ContentSanskrit = "धर्मक्षेत्रे" }
			};
			_mockRepo.Setup(r => r.SearchVersesBySanskritFTSAsync(query, maxRows, scriptureId)).ReturnsAsync(expected);

			// Act
			var result = await _provider.SearchVersesBySanskritFTSAsync(query, maxRows, scriptureId);

			// Assert
			Assert.NotNull(result);
			Assert.Single(result);
			Assert.Equal(201, result.First().VerseId);
			_mockRepo.Verify(r => r.SearchVersesBySanskritFTSAsync(query, maxRows, scriptureId), Times.Once);
		}

		[Theory]
		[InlineData(TranslationLanguage.English, "english")]
		[InlineData(TranslationLanguage.Hindi, "hindi")]
		[InlineData(TranslationLanguage.Both, "both")]
		public async Task SearchVersesByTranslationFTSAsync_Should_Call_Repository_Converting_Enum_To_Lowercase_String(TranslationLanguage inputLang, string expectedLangStr)
		{
			// Arrange
			string query = "righteousness";
			int maxRows = 15;
			int? scriptureId = null;
			var expected = new List<DbVerseSearchResult>();
			_mockRepo.Setup(r => r.SearchVersesByTranslationFTSAsync(query, maxRows, expectedLangStr, scriptureId)).ReturnsAsync(expected);

			// Act
			var result = await _provider.SearchVersesByTranslationFTSAsync(query, maxRows, inputLang, scriptureId);

			// Assert
			Assert.NotNull(result);
			_mockRepo.Verify(r => r.SearchVersesByTranslationFTSAsync(query, maxRows, expectedLangStr, scriptureId), Times.Once);
		}
	}
}
