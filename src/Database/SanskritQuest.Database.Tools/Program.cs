using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SanskritQuest.Database.Tools.Ingestion;
using SanskritQuest.Database.Tools.Repository;

namespace SanskritQuest.Database.Tools
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Set output encoding to UTF-8 to correctly print Devanagari (Sanskrit/Hindi) characters
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Initialize Configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("PostgreSQL Connection string 'DefaultConnection' not found.");

            // Register PostgreSQL Insight Db Provider (keep for backward compatibility if other tools use it)
            try
            {
                Insight.Database.Providers.PostgreSQL.PostgreSQLInsightDbProvider.RegisterProvider();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Warning] Failed to register Insight Db Provider: {ex.Message}");
            }

            // Setup Dependency Injection Container
            var services = new ServiceCollection();
            ConfigureServices(services, configuration, connectionString);
            var serviceProvider = services.BuildServiceProvider();

            // Resolve Ingestion Orchestrator
            var orchestrator = serviceProvider.GetRequiredService<IngestionOrchestrator>();

            // Run Interactive CLI Loop
            while (true)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("==================================================================");
                Console.WriteLine("             SANSKRIT QUEST SCRIPTURE INGESTER                   ");
                Console.WriteLine("==================================================================");
                Console.ResetColor();
                Console.WriteLine("Available options for ingestion: 'Bhagavad_Gita', 'Valmiki Ramayana', 'All'");
                Console.Write("Enter scripture to ingest (or 'exit' to quit): ");
                
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                string normalizedInput = input.Trim();
                if (normalizedInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Exiting scripture ingester. Good bye!");
                    Console.ResetColor();
                    break;
                }

                try
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[INGESTION START] Initializing pipeline for '{normalizedInput}'...");
                    Console.ResetColor();

                    await orchestrator.IngestScriptureAsync(normalizedInput);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[SUCCESS] Scripture '{normalizedInput}' ingested successfully!");
                    Console.ResetColor();

                    // Verification phase
                    string verificationTarget = ResolveScriptureCodeForVerification(normalizedInput);
                    if (verificationTarget == "Bhagavad_Gita" || verificationTarget == "All")
                    {
                        await VerifyScriptureDataAsync(connectionString, "Bhagavad_Gita", 1);
                    }
                    if (verificationTarget == "Valmiki_Ramayana" || verificationTarget == "All")
                    {
                        await VerifyScriptureDataAsync(connectionString, "Valmiki_Ramayana", 2);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ERROR] Ingestion failed for '{normalizedInput}':");
                    Console.WriteLine(ex.Message);
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Details: {ex.InnerException.Message}");
                    }
                    Console.ResetColor();
                }
            }
        }

        private static string ResolveScriptureCodeForVerification(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            var normalized = input.Trim().ToLowerInvariant();

            if (normalized.Contains("gita") || normalized == "geeta" || normalized == "bg" || normalized == "bhagavad_gita")
            {
                return "Bhagavad_Gita";
            }
            if (normalized.Contains("ramayan") || normalized == "vr" || normalized == "valmiki ramayana" || normalized == "valmiki_ramayana")
            {
                return "Valmiki_Ramayana";
            }
            if (normalized == "all")
            {
                return "All";
            }

            return input;
        }

        private static async Task VerifyScriptureDataAsync(string connectionString, string scriptureCode, int scriptureId)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"==================================================================");
            Console.WriteLine($"   VERIFICATION FOR SCRIPTURE: {scriptureCode} (ID: {scriptureId})");
            Console.WriteLine($"==================================================================");
            Console.ResetColor();

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();

                // Query 1: Count Hierarchy Nodes
                string hierarchyCountQuery = "SELECT COUNT(*) FROM scripture.hierarchy WHERE scripture_id = @scriptureId;";
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"Executing query: {hierarchyCountQuery}");
                Console.ResetColor();

                using var cmd1 = new NpgsqlCommand(hierarchyCountQuery, conn);
                cmd1.Parameters.AddWithValue("scriptureId", scriptureId);
                long hierarchyCount = Convert.ToInt64(await cmd1.ExecuteScalarAsync());

                // Query 2: Count Verses
                string verseCountQuery = @"
                    SELECT COUNT(*) FROM scripture.verses 
                    WHERE hierarchy_id IN (SELECT hierarchy_id FROM scripture.hierarchy WHERE scripture_id = @scriptureId);";
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"Executing query: {verseCountQuery}");
                Console.ResetColor();

                using var cmd2 = new NpgsqlCommand(verseCountQuery, conn);
                cmd2.Parameters.AddWithValue("scriptureId", scriptureId);
                long verseCount = Convert.ToInt64(await cmd2.ExecuteScalarAsync());

                // Query 3: Select sample verse
                string sampleVerseQuery = @"
                    SELECT verse_number, content_sanskrit, verse_data::text 
                    FROM scripture.verses 
                    WHERE hierarchy_id IN (SELECT hierarchy_id FROM scripture.hierarchy WHERE scripture_id = @scriptureId) 
                    ORDER BY verse_id ASC LIMIT 1;";
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"Executing query: {sampleVerseQuery}");
                Console.ResetColor();

                using var cmd3 = new NpgsqlCommand(sampleVerseQuery, conn);
                cmd3.Parameters.AddWithValue("scriptureId", scriptureId);
                
                string? verseNumber = null;
                string? sanskritText = null;
                string? verseDataJson = null;

                using (var reader = await cmd3.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        verseNumber = reader.GetString(0);
                        sanskritText = reader.GetString(1);
                        verseDataJson = reader.GetString(2);
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[VERIFICATION SUCCESS]");
                Console.ResetColor();
                Console.WriteLine($"  Hierarchy Node Count: {hierarchyCount}");
                Console.WriteLine($"  Verse Count:          {verseCount}");
                if (verseNumber != null)
                {
                    Console.WriteLine($"  Sample Verse:         {verseNumber}");
                    Console.WriteLine($"  Sanskrit Content:     {sanskritText}");
                    Console.WriteLine($"  Verse JSON Data:      {verseDataJson}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  No verses found in the database for this scripture.");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[VERIFICATION FAILURE] Verification queries failed for '{scriptureCode}':");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            Console.WriteLine("==================================================================");
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration, string connectionString)
        {
            // Add Configuration singleton
            services.AddSingleton<IConfiguration>(configuration);

            // Add Repository Layer with Connection String
            services.AddSingleton<IIngestionRepository>(new IngestionRepository(connectionString));

            // Add IScriptureIngesters
            services.AddTransient<IScriptureIngester, GitaExcelIngester>();
            services.AddTransient<IScriptureIngester, RamayanaExcelIngester>();

            // Add Orchestrator
            services.AddTransient<IngestionOrchestrator>();
        }
    }
}
