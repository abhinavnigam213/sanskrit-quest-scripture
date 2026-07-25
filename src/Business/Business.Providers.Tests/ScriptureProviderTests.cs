using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SanskritQuest.Business.Contracts;
using SanskritQuest.Business.Providers;
using SanskritQuest.Common.Contracts;
using SanskritQuest.Data.Contracts;
using Xunit;

namespace SanskritQuest.Business.Providers.Tests;

public class ScriptureProviderTests
{
	private readonly Mock<ILocalDataSetsProvider> _mockLocalDataSetsProvider;
	private readonly Mock<IScripturesDataProvider> _mockScripturesDataProvider;
	private readonly ScriptureProvider _provider;

	public ScriptureProviderTests()
	{
		_mockLocalDataSetsProvider = new Mock<ILocalDataSetsProvider>();
		_mockScripturesDataProvider = new Mock<IScripturesDataProvider>();
		_provider = new ScriptureProvider(_mockLocalDataSetsProvider.Object, _mockScripturesDataProvider.Object);
	}

	[Fact]
	public void GetPopularScriptures_ShouldReturnMappedBusinessScriptures()
	{
		// Arrange
		var popular = new List<Data.Contracts.Scripture>
		{
			new Data.Contracts.Scripture("1", "Bhagavad Gita", "BG", "Smriti", "1.1", "dharmaksetre", "In holy field", "धर्मक्षेत्र में")
		};
		_mockLocalDataSetsProvider.Setup(p => p.PopularScriptures).Returns(popular);

		// Act
		var result = _provider.GetPopularScriptures();

		// Assert
		Assert.NotNull(result);
		Assert.Single(result);
		var item = result.First();
		Assert.Equal(1, item.ScriptureId);
		Assert.Equal("BG", item.Code);
		Assert.Equal("BhagavadGita", item.EnumName);
		Assert.Equal("Bhagavad Gita", item.Titles.En);
	}

	[Fact]
	public async Task GetAllScripturesAsync_ShouldReturnAllScriptures()
	{
		// Arrange
		var dbScriptures = new List<DbScripture>
		{
			new DbScripture
			{
				ScriptureId = 2,
				Code = "VR",
				Titles = new LocalizedTitles { En = "Valmiki Ramayana" },
				Description = new LocalizedDescription { En = "Valmiki's Ramayana" }
			}
		};
		_mockScripturesDataProvider
			.Setup(p => p.GetAllScripturesAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(dbScriptures);

		// Act
		var result = await _provider.GetAllScripturesAsync();

		// Assert
		Assert.NotNull(result);
		Assert.Single(result);
		var item = result.First();
		Assert.Equal(2, item.ScriptureId);
		Assert.Equal("VR", item.Code);
		Assert.Equal("ValmikiRamayana", item.EnumName);
	}

	[Fact]
	public async Task GetScriptureDetailsAsync_ShouldBuildHierarchyTreeAndLabels()
	{
		// Arrange
		int scriptureId = 2; // Valmiki Ramayana
		var dbDetails = new List<ScriptureDetail>
		{
			new ScriptureDetail
			{
				ScriptureId = 2,
				ScriptureCode = "VR",
				HierarchyId = 100,
				ParentId = null,
				LocalLabel = "Bala Kanda",
				Path = "1",
				NodeType = HierarchyNodeType.Kanda,
				SequenceNumber = 1
			},
			new ScriptureDetail
			{
				ScriptureId = 2,
				ScriptureCode = "VR",
				HierarchyId = 101,
				ParentId = 100,
				LocalLabel = "Sarga 1",
				Path = "1/1",
				NodeType = HierarchyNodeType.Sarga,
				SequenceNumber = 1
			}
		};
		_mockScripturesDataProvider
			.Setup(p => p.GetScriptureDetailsAsync(scriptureId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(dbDetails);

		// Act
		var result = await _provider.GetScriptureDetailsAsync(scriptureId);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("VR", result!.Code);
		Assert.Equal("ValmikiRamayana", result.EnumName);
		Assert.Single(result.Hierarchy);

		var kandaNode = result.Hierarchy.First();
		Assert.Equal("Bala Kanda", kandaNode.LocalLabel);
		Assert.Equal("ValmikiRamayana.Bala Kanda", kandaNode.FullHierarchyLabel);
		Assert.Single(kandaNode.Children);

		var sargaNode = kandaNode.Children.First();
		Assert.Equal("Sarga 1", sargaNode.LocalLabel);
		Assert.Equal("ValmikiRamayana.Bala Kanda.Sarga 1", sargaNode.FullHierarchyLabel);
	}

	[Fact]
	public async Task GetVerseDetailsAsync_ShouldBuildVerseDetailsAndHierarchyAndTransliteration()
	{
		// Arrange
		int verseId = 50;
		var verseDetail = new VerseDetail
		{
			VerseId = verseId,
			VerseNumber = "1",
			VerseType = VerseType.Shloka,
			ContentSanskrit = "धर्मक्षेत्रे कुरुक्षेत्रे",
			ScriptureId = 1,
			ScriptureTitles = new LocalizedTitles { En = "Bhagavad Gita" },
			HierarchyId = 100,
			VerseData = new VerseData
			{
				TranslationEn = "On the field of Dharma, Kurukṣetra",
				TranslationHi = "धर्म भूमि कुरुक्षेत्र पर"
			},
			WordByWordBreakdown = new List<DbWordBreakdownItem>
			{
				new DbWordBreakdownItem { SanskritWord = "धर्मक्षेत्रे", TranslationEn = "on the field of Dharma" }
			}
		};

		var scriptureDetails = new List<ScriptureDetail>
		{
			new ScriptureDetail
			{
				ScriptureId = 1,
				ScriptureCode = "BG",
				HierarchyId = 100,
				ParentId = null,
				LocalLabel = "Chapter 1",
				Path = "1",
				NodeType = HierarchyNodeType.Adhyaya,
				SequenceNumber = 1,
				HierarchyTitles = new LocalizedTitles { Sa = "अर्जुनविषादयोगः" }
			}
		};

		_mockScripturesDataProvider
			.Setup(p => p.GetVerseDetailsAsync(verseId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(verseDetail);

		_mockScripturesDataProvider
			.Setup(p => p.GetScriptureDetailsAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(scriptureDetails);

		// Act
		var result = await _provider.GetVerseDetailsAsync(verseId);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Bhagavad Gita", result!.ScriptureName);
		Assert.Equal("धर्मक्षेत्रे कुरुक्षेत्रे", result.SanskritShloka);
		// Transliteration is resolved via common utility:
		Assert.Contains("dharmakṣetre", result.Transliteration);
		Assert.Equal("dharmakṣetre", result.WordByWordBreakdown.First().Transliteration);

		// Hierarchy should be built successfully
		Assert.Equal(2, result.Hierarchy.Count);
		var level1 = result.Hierarchy.First();
		Assert.Equal("Adhyaya", level1.LevelType);
		Assert.Equal("अर्जुनविषादयोगः", level1.LevelNameSanskrit);

		var level2 = result.Hierarchy.Last();
		Assert.Equal("Shloka", level2.LevelType);
		Assert.Equal("श्लोक 1", level2.LevelNameSanskrit);
	}
}
