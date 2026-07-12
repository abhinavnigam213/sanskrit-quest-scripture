using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Insight.Database;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SanskritQuest.Data.Contracts;

namespace SanskritQuest.Database.Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("PostgreSQL Connection string 'DefaultConnection' not found.");

            // Register PostgreSQL Insight Db Provider
            Insight.Database.Providers.PostgreSQL.PostgreSQLInsightDbProvider.RegisterProvider();

            string jsonPath = configuration.GetValue<string>("DataSources:JsonSourcePath") ?? "";
            string csvPath = configuration.GetValue<string>("DataSources:CsvSourcePath") ?? "";
            string sqliteConnectionString = configuration.GetValue<string>("DataSources:SqliteSourceConnection") ?? "";

            if (args.Length == 0)
            {
                ShowUsage();
                return;
            }

            string action = args[0].ToLower();

            try
            {
                switch (action)
                {
                    case "--import-json":
                        Console.WriteLine($"Importing data from JSON source: {jsonPath}");
                        ImportFromJson(jsonPath, connectionString);
                        break;

                    case "--import-csv":
                        Console.WriteLine($"Importing data from CSV source: {csvPath}");
                        ImportFromCsv(csvPath, connectionString);
                        break;

                    case "--import-sqlite":
                        Console.WriteLine($"Importing data from SQLite source...");
                        ImportFromSqlite(sqliteConnectionString, connectionString);
                        break;

                    case "--export-json":
                        Console.WriteLine($"Exporting data to JSON target: {jsonPath}");
                        ExportToJson(jsonPath, connectionString);
                        break;

                    default:
                        Console.WriteLine($"Unknown argument: {args[0]}");
                        ShowUsage();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Operation failed: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
            }
        }

        static void ShowUsage()
        {
            Console.WriteLine("SanskritQuest Database Import/Export Tool");
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run --project SanskritQuest.Database.Tools -- --import-json");
            Console.WriteLine("  dotnet run --project SanskritQuest.Database.Tools -- --import-csv");
            Console.WriteLine("  dotnet run --project SanskritQuest.Database.Tools -- --import-sqlite");
            Console.WriteLine("  dotnet run --project SanskritQuest.Database.Tools -- --export-json");
        }

        private static void ImportFromJson(string filePath, string targetDbConnection)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"JSON file not found at: {filePath}");
            }

            string jsonContent = File.ReadAllText(filePath);
            var scriptures = JsonSerializer.Deserialize<List<Scripture>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (scriptures == null || scriptures.Count == 0)
            {
                Console.WriteLine("No scriptures found in the JSON file to import.");
                return;
            }

            Console.WriteLine($"Parsed {scriptures.Count} scriptures from JSON. Writing to PostgreSQL...");
            BulkUpsertScriptures(scriptures, targetDbConnection);
            Console.WriteLine("JSON import completed successfully.");
        }

        private static void ImportFromCsv(string filePath, string targetDbConnection)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"CSV file not found at: {filePath}");
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);
            var scriptures = csv.GetRecords<Scripture>().ToList();

            if (scriptures.Count == 0)
            {
                Console.WriteLine("No scriptures found in the CSV file to import.");
                return;
            }

            Console.WriteLine($"Parsed {scriptures.Count} scriptures from CSV. Writing to PostgreSQL...");
            BulkUpsertScriptures(scriptures, targetDbConnection);
            Console.WriteLine("CSV import completed successfully.");
        }

        private static void ImportFromSqlite(string sqliteConnString, string targetDbConnection)
        {
            if (string.IsNullOrWhiteSpace(sqliteConnString))
            {
                throw new ArgumentException("SQLite connection string is not configured.");
            }

            var scriptures = new List<Scripture>();
            using (var sqliteConn = new SqliteConnection(sqliteConnString))
            {
                sqliteConn.Open();
                
                // Select columns from the source SQLite database scriptures table
                using var cmd = new SqliteCommand(@"
                    SELECT id, title, source, category, verse, 
                           transliteration_default, translation_default_english, translation_default_hindi 
                    FROM scriptures", sqliteConn);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    scriptures.Add(new Scripture(
                        Id: reader.GetString(0),
                        Title: reader.GetString(1),
                        Source: reader.GetString(2),
                        Category: reader.GetString(3),
                        Verse: reader.GetString(4),
                        TransliterationDefault: reader.IsDBNull(5) ? "" : reader.GetString(5),
                        TranslationDefaultEnglish: reader.IsDBNull(6) ? "" : reader.GetString(6),
                        TranslationDefaultHindi: reader.IsDBNull(7) ? "" : reader.GetString(7)
                    ));
                }
            }

            if (scriptures.Count == 0)
            {
                Console.WriteLine("No scriptures found in SQLite database to import.");
                return;
            }

            Console.WriteLine($"Retrieved {scriptures.Count} scriptures from SQLite. Writing to PostgreSQL...");
            BulkUpsertScriptures(scriptures, targetDbConnection);
            Console.WriteLine("SQLite import completed successfully.");
        }

        private static void ExportToJson(string targetPath, string sourceDbConnection)
        {
            Console.WriteLine("Reading scriptures from PostgreSQL database...");
            using var connection = new NpgsqlConnection(sourceDbConnection);
            var repository = connection.As<IScriptureRepository>();
            var scriptures = repository.GetAllScriptures();

            Console.WriteLine($"Retrieved {scriptures.Count} scriptures. Writing to JSON file...");
            
            var directory = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string jsonContent = JsonSerializer.Serialize(scriptures, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(targetPath, jsonContent);
            Console.WriteLine($"Export completed successfully. Saved to: {targetPath}");
        }

        private static void BulkUpsertScriptures(List<Scripture> scriptures, string targetDbConnection)
        {
            using var connection = new NpgsqlConnection(targetDbConnection);
            connection.Open();
            
            var repository = connection.As<IScriptureRepository>();

            using var transaction = connection.BeginTransaction();
            try
            {
                foreach (var scripture in scriptures)
                {
                    repository.InsertOrUpdateScripture(scripture);
                }
                transaction.Commit();
                Console.WriteLine($"Upserted {scriptures.Count} scriptures in a single transaction transaction.");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    /// <summary>
    /// Inline interface mapped using Insight.Database.
    /// Maps custom SQL statements directly to the target schema structure.
    /// </summary>
    public interface IScriptureRepository
    {
        [Sql(@"
            INSERT INTO scriptures (
                id, title, source, category, verse, 
                transliteration_default, translation_default_english, translation_default_hindi
            ) VALUES (
                @Id, @Title, @Source, @Category, @Verse, 
                @TransliterationDefault, @TranslationDefaultEnglish, @TranslationDefaultHindi
            ) ON CONFLICT (id) DO UPDATE SET 
                title = EXCLUDED.title, 
                source = EXCLUDED.source, 
                category = EXCLUDED.category, 
                verse = EXCLUDED.verse, 
                transliteration_default = EXCLUDED.transliteration_default, 
                translation_default_english = EXCLUDED.translation_default_english, 
                translation_default_hindi = EXCLUDED.translation_default_hindi")]
        void InsertOrUpdateScripture(Scripture scripture);

        [Sql(@"
            SELECT 
                id AS Id, 
                title AS Title, 
                source AS Source, 
                category AS Category, 
                verse AS Verse, 
                transliteration_default AS TransliterationDefault, 
                translation_default_english AS TranslationDefaultEnglish, 
                translation_default_hindi AS TranslationDefaultHindi 
            FROM scriptures")]
        List<Scripture> GetAllScriptures();
    }
}
