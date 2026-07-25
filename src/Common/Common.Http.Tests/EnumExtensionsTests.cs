using Xunit;
using SanskritQuest.Common.Contracts;
using SanskritQuest.Common.Utilities;

namespace SanskritQuest.Common.Http.Tests
{
	public class EnumExtensionsTests
	{
		[Fact]
		public void GetDatabaseId_ShouldReturnCorrectId_ForDecoratedEnums()
		{
			// Arrange
			var sect = Sect.Shaivism;
			var deity = Deity.Krishna;
			var scripture = ScriptureType.BhagavadGita;

			// Act
			int? sectId = sect.GetDatabaseId();
			int? deityId = deity.GetDatabaseId();
			int? scriptureId = scripture.GetDatabaseId();

			// Assert
			Assert.Equal(2, sectId);
			Assert.Equal(2, deityId);
			Assert.Equal(1, scriptureId);
		}

		[Fact]
		public void GetDatabaseId_ShouldReturnNull_ForNonDecoratedEnums()
		{
			// Arrange
			var lang = TranslationLanguage.English;

			// Act
			int? langId = lang.GetDatabaseId();

			// Assert
			Assert.Null(langId);
		}

		[Fact]
		public void GetEnumByDatabaseId_ShouldReturnCorrectEnum_ForValidId()
		{
			// Arrange & Act
			var sect = 3.GetEnumByDatabaseId<Sect>();
			var deity = 12.GetEnumByDatabaseId<Deity>();
			var scripture = 2.GetEnumByDatabaseId<ScriptureType>();

			// Assert
			Assert.Equal(Sect.Shaktism, sect);
			Assert.Equal(Deity.Rama, deity);
			Assert.Equal(ScriptureType.ValmikiRamayana, scripture);
		}

		[Fact]
		public void GetEnumByDatabaseId_ShouldReturnNull_ForInvalidId()
		{
			// Arrange & Act
			var sect = 99.GetEnumByDatabaseId<Sect>();

			// Assert
			Assert.Null(sect);
		}
	}
}
