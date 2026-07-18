using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SanskritQuest.Common.Configuration;
using SanskritQuest.Common.Utilities;
using SanskritQuest.Data.Contracts;
using SanskritQuest.Data.Providers.Extensions;

namespace Data.TestConsole
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			// Set console output encoding to UTF8 for rendering Sanskrit and Hindi text
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			// Build configuration to read appSettings.json
			IConfiguration configuration = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
				.Build();

			var services = new ServiceCollection();
			// Bind configuration objects used by data providers
			services.AddCommonConfiguration(configuration);
			services.AddDataProviders();

			IServiceProvider serviceProvider = services.BuildServiceProvider();

			TestDbConnection(serviceProvider);

			await TestSearchAsync(serviceProvider);

			await RunScriptureProviderTestsAsync(serviceProvider);

			Console.WriteLine("\nDone.");
		}

		private async static Task TestSearchAsync(IServiceProvider serviceProvider)
		{
			try
			{
				Console.WriteLine("\n--- Testing Dictionary Provider ---");
				var dictionaryProvider = serviceProvider.GetRequiredService<IDictionaryDataProvider>();
				var results = await dictionaryProvider.ExactSearchAsync("परिहारः", SanskritScheme.Devanagari);
				foreach (var item in results)
				{
					var entry = $"English: {item?.EnglishMeanings}, Hindi: {item?.HindiMeanings}, Grammar: {item?.LexicalInfo}";
					Console.WriteLine(entry);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Search test failed: {ex.Message}");
			}
		}

		private async static Task RunScriptureProviderTestsAsync(IServiceProvider serviceProvider)
		{
			try
			{
				Console.WriteLine("\n--- Testing Scripture Provider ---");
				var scriptureProvider = serviceProvider.GetRequiredService<IScripturesDataProvider>();

				// 1. Test GetAllScripturesAsync
				var scriptures = await ScripturesProviderTest.TestGetAllScripturesAsync(scriptureProvider);

				if (scriptures.Any())
				{
					var targetScripture = scriptures.First();
					
					// 2. Test GetScriptureDetailsAsync
					var details = await ScripturesProviderTest.TestGetScriptureDetailsAsync(scriptureProvider, targetScripture.ScriptureId, targetScripture.Code);

					// Find a hierarchy node that has direct verses to test verse ID retrieval
					var nodeWithVerses = details.FirstOrDefault(d => d.DirectVerseCount > 0);
					if (nodeWithVerses != null)
					{
						// 3. Test GetAllVersesIdForHierarchyAsync
						var verseIds = await ScripturesProviderTest.TestGetAllVersesIdForHierarchyAsync(scriptureProvider, nodeWithVerses.HierarchyId, nodeWithVerses.LocalLabel);

						if (verseIds.Any())
						{
							var targetVerseId = verseIds.First().VerseId;
							
							// 4. Test GetVerseDetailsAsync
							await ScripturesProviderTest.TestGetVerseDetailsAsync(scriptureProvider, targetVerseId);
						}
					}
				}

				// 5. Test SearchVersesBySanskritFTSAsync
				await ScripturesProviderTest.TestSearchVersesBySanskritFTSAsync(scriptureProvider, "धर्म");

				// 6. Test SearchVersesByTranslationFTSAsync
				await ScripturesProviderTest.TestSearchVersesByTranslationFTSAsync(scriptureProvider, "righteousness");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Scripture provider test execution failed: {ex.Message}");
			}
		}

		private static void TestDbConnection(IServiceProvider serviceProvider)
		{
			try
			{
				Console.WriteLine("--- Testing Connection ---");
				var factory = serviceProvider.GetRequiredService<IDbConnectionFactory>();
				using IDbConnection conn = factory.GetDefaultDbConnection();
				conn.Open();
				Console.WriteLine("Connection opened successfully.");

				using IDbCommand cmd = conn.CreateCommand();
				cmd.CommandText = "SELECT 1";
				var scalar = cmd.ExecuteScalar();
				Console.WriteLine($"SELECT 1 result: {scalar}");

				using IDbCommand cmd2 = conn.CreateCommand();
				cmd2.CommandText = "SELECT version()";
				var ver = cmd2.ExecuteScalar();
				Console.WriteLine($"Server version: {ver}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"DB test failed: {ex.Message}");
			}
		}
	}
}
