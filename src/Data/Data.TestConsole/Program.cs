using System.Data;
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
			// Build configuration to read appsettings.json
			IConfiguration configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();

			var services = new ServiceCollection();
			// Bind configuration objects used by data providers
			services.AddCommonConfiguration(configuration);
			services.AddDataProviders();

			IServiceProvider serviceProvider = services.BuildServiceProvider();

			TestDbConnection(serviceProvider);

			await TestSearchAsync(serviceProvider);

			Console.WriteLine("Done.");
		}

		private async static Task TestSearchAsync(IServiceProvider serviceProvider)
		{
			try
			{
				var dictionaryProvider = serviceProvider.GetRequiredService<IDictionaryDataProvider>();
				var results = await dictionaryProvider.ExactSearchAsync("परिहारः", SanskritScheme.Devanagari);
				foreach (var item in results)
				{
					var entry = $"English: {item?.EnglishMeanings}, Hindi: {item?.HindiMeanings}, Grammer: {item?.LexicalInfo}";
					Console.WriteLine(entry);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Search test failed: {ex.Message}");
			}
		}

		private static void TestDbConnection(IServiceProvider serviceProvider)
		{
			try
			{
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
