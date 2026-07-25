using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SanskritQuest.Common.Configuration;
using SanskritQuest.Common.Contracts;
using SanskritQuest.Data.Providers.Extensions;
using SanskritQuest.Services.AIService.Contracts;
using Microsoft.Extensions.AI;

namespace SanskritQuest.Services.AIService.TestConsole
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			// Set console output encoding to UTF8 to correctly print Sanskrit, IAST, and Hindi text
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			Console.WriteLine("==================================================");
			Console.WriteLine("         SanskritQuest AI Service Test Console    ");
			Console.WriteLine("==================================================\n");

			IConfiguration configuration = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();

			// Setup DI
			var services = new ServiceCollection();

			// Add configuration and core settings
			services.AddCommonConfiguration(configuration);

			// Add Data Providers (required for ILocalDataSetsProvider in SanskritScholarAIService)
			services.AddDataProviders();

			// Add AI Services
			services.AddAIServices(configuration);

			var serviceProvider = services.BuildServiceProvider();

			// Print out the active provider from configuration
			var aiSettings = serviceProvider.GetRequiredService<AISettings>();
			Console.WriteLine($"Active AI Provider: {aiSettings.ActiveProvider}");
			if (aiSettings.Providers.TryGetValue(aiSettings.ActiveProvider, out var providerSettings))
			{
				Console.WriteLine($"Model ID: {providerSettings.ModelId}");
				Console.WriteLine($"Endpoint: {providerSettings.Endpoint}");
				var hasKey = !string.IsNullOrEmpty(providerSettings.ApiKey) ||
							 !string.IsNullOrEmpty(Environment.GetEnvironmentVariable($"{aiSettings.ActiveProvider.ToUpper()}_API_KEY"));
				Console.WriteLine($"API Key Configured: {(hasKey ? "Yes" : "No (Will use fallback/offline mode if needed)")}");
			}
			Console.WriteLine();

			// Resolve services
			var translationService = serviceProvider.GetService<ISanskritTranslationAIService>();
			var chatClient = serviceProvider.GetService<IChatClient>();
			// var scholarService = serviceProvider.GetService<ISanskritScholarAIService>();

			if (translationService == null)
			{
				Console.WriteLine("Error: ISanskritTranslationAIService could not be resolved.");
				return;
			}

			// if (scholarService == null)
			// {
			// 	Console.WriteLine("Error: ISanskritScholarAIService could not be resolved.");
			// 	return;
			// }

			// Run tests
			if (chatClient != null)
			{
				await RunTranslationServiceTestsAsync(translationService);
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("========================================================================");
				Console.WriteLine(" WARNING: AI Chat Client (IChatClient) is not configured.");
				Console.WriteLine(" Please configure your API key to test the live translation endpoints.");
				Console.WriteLine();
				Console.WriteLine(" To configure a provider:");
				Console.WriteLine(" 1. Open 'Services/Services.AIService.TestConsole/appsettings.Development.json'");
				Console.WriteLine(" 2. Replace the placeholder for your active provider with a valid API key:");
				Console.WriteLine("    e.g. \"ApiKey\": \"your-actual-api-key-here\"");
				Console.WriteLine(" 3. Alternatively, set the environment variable: e.g. GEMINI_API_KEY");
				Console.WriteLine("========================================================================");
				Console.ResetColor();
				Console.WriteLine();
			}
			// await RunScholarServiceTestsAsync(scholarService);

			Console.WriteLine("\n==================================================");
			Console.WriteLine("Tests completed. Press any key to exit.");
			Console.WriteLine("==================================================");
		}

		private static async Task RunTranslationServiceTestsAsync(ISanskritTranslationAIService translationService)
		{
			Console.WriteLine("--------------------------------------------------");
			Console.WriteLine(" Testing ISanskritTranslationAIService            ");
			Console.WriteLine("--------------------------------------------------");

			const string sampleShloka = "धर्मक्षेत्रे कुरुक्षेत्रे समवेता युयुत्सवः।\nमामकाः पाण्डवाश्चैव किमकुर्वत सञ्जय॥";
			Console.WriteLine($"Translating Shloka:\n{sampleShloka}\n");

			try
			{
				Console.WriteLine("1. Single Translation (English and Hindi)...");
				var result = await translationService.TranslateShlokaAsync(1, sampleShloka, target: TranslationLanguage.Both);
				if (result != null)
				{
					Console.WriteLine($"[Success] Index: {result.Index}");
					Console.WriteLine($"English Translation:\n{result.TranslationEn}");
					Console.WriteLine($"Hindi Translation:\n{result.TranslationHi}");
				}
				else
				{
					Console.WriteLine("[Failed] Returned null translation result.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Error] Single translation failed: {ex.Message}");
			}

			Console.WriteLine();

			try
			{
				Console.WriteLine("2. Batch Translation (English)...");
				var batch = new List<(int index, string sanskrit, string? english)>
				{
					(1, "यदा यदा हि धर्मस्य ग्लानिर्भवति भारत।", null),
					(2, "अभ्युत्थानमधर्मस्य तदात्मानं सृजाम्यहम्॥", null)
				};

				var results = await translationService.TranslateShlokasBatchAsync(batch, target: TranslationLanguage.English);
				if (results != null)
				{
					foreach (var res in results)
					{
						Console.WriteLine($"Index {res.Index} English Translation: {res.TranslationEn}");
					}
				}
				else
				{
					Console.WriteLine("[Failed] Returned null batch result.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Error] Batch translation failed: {ex.Message}");
			}
			Console.WriteLine();
		}

		/*
		private static async Task RunScholarServiceTestsAsync(ISanskritScholarAIService scholarService)
		{
			Console.WriteLine("--------------------------------------------------");
			Console.WriteLine(" Testing ISanskritScholarAIService               ");
			Console.WriteLine("--------------------------------------------------");

			const string textToTranslate = "कर्मण्येवाधिकारस्ते मा फलेषु कदाचन।";
			Console.WriteLine($"Text for Analysis/Transliteration: {textToTranslate}\n");

			try
			{
				Console.WriteLine("1. Translating Text...");
				var result = await scholarService.TranslateTextAsync(textToTranslate, "sanskrit", "english", scriptureContext: null);
				Console.WriteLine($"[Success] Translated Text: {result.TranslatedText}");
				Console.WriteLine($"Explanation: {result.Explanation}");
				Console.WriteLine($"Is Fallback: {result.IsFallback}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Error] TranslateTextAsync failed: {ex.Message}");
			}

			Console.WriteLine();

			try
			{
				Console.WriteLine("2. Transliterating Text (Devanagari to IAST)...");
				var result = await scholarService.TransliterateTextAsync(textToTranslate, "devanagari", "iast");
				Console.WriteLine($"[Success] Transliterated: {result.TransliteratedText}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Error] TransliterateTextAsync failed: {ex.Message}");
			}

			Console.WriteLine();

			try
			{
				Console.WriteLine("3. Analyzing Scripture...");
				var result = await scholarService.AnalyzeScriptureAsync(textToTranslate, sourceContext: null);
				Console.WriteLine($"[Success] Identified Source: {result.IdentifiedSource}");
				Console.WriteLine($"IAST: {result.TransliterationIAST}");
				Console.WriteLine($"English Translation: {result.TranslationEnglish}");
				Console.WriteLine($"Hindi Translation: {result.TranslationHindi}");
				Console.WriteLine($"Meter: {result.PoeticMeter}");
				Console.WriteLine($"Is Fallback: {result.IsFallback}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Error] AnalyzeScriptureAsync failed: {ex.Message}");
			}
			Console.WriteLine();
		}
		*/
	}
}
