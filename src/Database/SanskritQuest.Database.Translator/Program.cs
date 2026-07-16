using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniExcelLibs;
using SanskritQuest.Database.Translation;

namespace SanskritQuest.Database.Translator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==================================================================");
            Console.WriteLine("             SANSKRIT SCRIPTURE TRANSLATOR (C#)                  ");
            Console.WriteLine("==================================================================");
            Console.ResetColor();

            // 1. Load Configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            bool disableTranslation = false;
            if (bool.TryParse(configuration["DisableTranslation"], out bool disableVal))
            {
                disableTranslation = disableVal;
            }

            string apiKey = configuration["GeminiApiKey"] ?? "";
            if (!disableTranslation && (string.IsNullOrWhiteSpace(apiKey) || apiKey == "PASTE_YOUR_GEMINI_API_KEY_HERE"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Error] GeminiApiKey is missing or set to the placeholder in appsettings.Development.json.");
                Console.WriteLine("Please populate the GeminiApiKey with a valid key from Google AI Studio.");
                Console.ResetColor();
                return;
            }

            string modelName = configuration["ModelName"] ?? "gemini-1.5-flash";
            string targetStr = configuration["Target"] ?? "Both";
            int batchSize = int.Parse(configuration["BatchSize"] ?? "20");
            int maxParallelRequests = int.Parse(configuration["MaxParallelRequests"] ?? "5");
            string csvFilePath = configuration["CsvFilePath"] ?? "../data-sets/scriptures/Valmiki_Ramayan_Shlokas_Translated.csv";
            string outputExcelFilePath = configuration["OutputExcelFilePath"] ?? "../data-sets/scriptures/Valmiki_Ramayan_Shlokas_Translated_Hindi.xlsx";
            string databaseFilePath = configuration["DatabaseFilePath"] ?? "translation_progress.db";

            if (!Enum.TryParse(targetStr, out TranslationTarget target))
            {
                target = TranslationTarget.Both;
            }

            // Resolve relative paths
            csvFilePath = ResolveFilePath(csvFilePath);
            string csvDir = Path.GetDirectoryName(csvFilePath) ?? AppContext.BaseDirectory;
            outputExcelFilePath = Path.GetFullPath(Path.Combine(csvDir, Path.GetFileName(outputExcelFilePath)));
            databaseFilePath = Path.GetFullPath(Path.Combine(csvDir, Path.GetFileName(databaseFilePath)));

            Console.WriteLine($"[Config] Disable Gemini Translation: {disableTranslation}");
            Console.WriteLine($"[Config] Model: {modelName}");
            Console.WriteLine($"[Config] Target Translation: {target}");
            Console.WriteLine($"[Config] Batch Size: {batchSize}");
            Console.WriteLine($"[Config] Parallelism: {maxParallelRequests}");
            Console.WriteLine($"[Config] Source CSV: {csvFilePath}");
            Console.WriteLine($"[Config] Output Excel: {outputExcelFilePath}");
            Console.WriteLine($"[Config] Progress DB: {databaseFilePath}");
            Console.WriteLine();

            if (!File.Exists(csvFilePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Error] Source CSV file not found: {csvFilePath}");
                Console.ResetColor();
                return;
            }

            // Ensure output directories exist
            var outputDir = Path.GetDirectoryName(outputExcelFilePath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // 2. Initialize SQLite Progress DB
            InitializeDatabase(databaseFilePath);

            // 3. Load Source CSV Data
            List<CsvRow> allRows;
            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { PrepareHeaderForMatch = args => args.Header.ToLower() }))
            {
                allRows = csv.GetRecords<CsvRow>().ToList();
            }

            // Assign indices to match exactly
            for (int i = 0; i < allRows.Count; i++)
            {
                allRows[i].Index = i;
            }

            // 4. Retrieve completed progress
            var completedIndices = GetCompletedIndices(databaseFilePath);
            Console.WriteLine($"[Progress] Loaded {allRows.Count} rows from CSV.");
            Console.WriteLine($"[Progress] Already translated in SQLite DB: {completedIndices.Count}");

            // 5. Partition remaining items into batches
            var remainingRows = allRows.Where(r => !completedIndices.Contains(r.Index)).ToList();
            var batches = new List<List<CsvRow>>();
            var currentBatch = new List<CsvRow>();

            foreach (var r in remainingRows)
            {
                currentBatch.Add(r);
                if (currentBatch.Count >= batchSize)
                {
                    batches.Add(currentBatch);
                    currentBatch = new List<CsvRow>();
                }
            }
            if (currentBatch.Count > 0)
            {
                batches.Add(currentBatch);
            }

            if (disableTranslation)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[Progress] Translation phase is disabled. Bypassing Gemini API calls.");
                Console.ResetColor();
            }
            else if (batches.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[Progress] All rows already translated in SQLite database!");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"[Progress] Remaining batches to process: {batches.Count} (Total remaining rows: {remainingRows.Count})");
                Console.WriteLine("Starting translations...");

                // 6. Process Batches in Parallel
                var services = new ServiceCollection();
                services.AddSanskritTranslationServices(configuration);
                using var serviceProvider = services.BuildServiceProvider();

                var translationService = serviceProvider.GetRequiredService<ISanskritTranslationService>();
                int completedCount = completedIndices.Count;
                var progressLock = new object();

                var parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = maxParallelRequests
                };

                await Parallel.ForEachAsync(batches, parallelOptions, async (batch, cancellationToken) =>
                {
                    var items = batch.Select(r => (index: r.Index, sanskrit: r.ShlokaText, english: (string?)r.Explanation)).ToList();
                    List<TranslationResult>? results = null;
                    int retries = 5;

                    for (int attempt = 1; attempt <= retries; attempt++)
                    {
                        try
                        {
                            results = await translationService.TranslateShlokasBatchAsync(items, cancellationToken);
                            if (results != null) break;
                        }
                        catch (Exception ex)
                        {
                            int delay = (int)Math.Pow(2, attempt) * 1000 + 500;
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"[Warning] Batch ending at index {batch[^1].Index} failed (attempt {attempt}/{retries}): {ex.Message}. Retrying in {delay}ms...");
                            Console.ResetColor();
                            await Task.Delay(delay, cancellationToken);
                        }
                    }

                    if (results != null)
                    {
                        SaveTranslations(databaseFilePath, results);
                        lock (progressLock)
                        {
                            completedCount += batch.Count;
                            double percent = (double)completedCount / allRows.Count * 100;
                            Console.WriteLine($"[Progress] Saved batch ending at index {batch[^1].Index}. Progress: {completedCount} / {allRows.Count} ({percent:F2}%)");
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[Error] Batch ending at index {batch[^1].Index} failed permanently.");
                        Console.ResetColor();
                    }
                });
            }

            // 7. Export SQLite Database back to the target Excel file
            Console.WriteLine("Exporting data from SQLite back into Excel...");
            var translationMap = GetTranslationMap(databaseFilePath);

            var exportList = new List<ExcelExportRow>();
            foreach (var row in allRows)
            {
                translationMap.TryGetValue(row.Index, out var trans);
                
                exportList.Add(new ExcelExportRow
                {
                    Kanda = row.Kanda,
                    Sarga = row.Sarga,
                    Shloka = row.Shloka,
                    ShlokaText = row.ShlokaText,
                    ExplanationEnglish = trans?.TranslationEn ?? row.Explanation,
                    ExplanationHindi = trans?.TranslationHi ?? ""
                });
            }

            try
            {
                if (File.Exists(outputExcelFilePath))
                {
                    File.Delete(outputExcelFilePath);
                }
                
                MiniExcel.SaveAs(outputExcelFilePath, exportList);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[SUCCESS] Excel spreadsheet generated successfully at:\n{outputExcelFilePath}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Error] Failed to write Excel file: {ex.Message}");
                Console.ResetColor();
            }
        }

        #region Database Utilities

        private static void InitializeDatabase(string dbPath)
        {
            using var conn = new SqliteConnection($"Data Source={dbPath}");
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS translations (
                    row_index INTEGER PRIMARY KEY,
                    translation_en TEXT,
                    translation_hi TEXT
                );";
            cmd.ExecuteNonQuery();
        }

        private static HashSet<int> GetCompletedIndices(string dbPath)
        {
            var indices = new HashSet<int>();
            using var conn = new SqliteConnection($"Data Source={dbPath}");
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT row_index FROM translations";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                indices.Add(reader.GetInt32(0));
            }
            return indices;
        }

        private static void SaveTranslations(string dbPath, List<TranslationResult> results)
        {
            using var conn = new SqliteConnection($"Data Source={dbPath}");
            conn.Open();
            using var transaction = conn.BeginTransaction();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT OR REPLACE INTO translations (row_index, translation_en, translation_hi)
                VALUES ($index, $en, $hi);";

            var indexParam = cmd.Parameters.Add("$index", SqliteType.Integer);
            var enParam = cmd.Parameters.Add("$en", SqliteType.Text);
            var hiParam = cmd.Parameters.Add("$hi", SqliteType.Text);

            foreach (var res in results)
            {
                indexParam.Value = res.Index;
                enParam.Value = (object?)res.TranslationEn ?? DBNull.Value;
                hiParam.Value = (object?)res.TranslationHi ?? DBNull.Value;
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        private static Dictionary<int, TranslationResult> GetTranslationMap(string dbPath)
        {
            var map = new Dictionary<int, TranslationResult>();
            using var conn = new SqliteConnection($"Data Source={dbPath}");
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT row_index, translation_en, translation_hi FROM translations";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int index = reader.GetInt32(0);
                string? en = reader.IsDBNull(1) ? null : reader.GetString(1);
                string? hi = reader.IsDBNull(2) ? null : reader.GetString(2);
                map[index] = new TranslationResult { Index = index, TranslationEn = en, TranslationHi = hi };
            }
            return map;
        }

        private static string ResolveFilePath(string configuredPath)
        {
            if (Path.IsPathRooted(configuredPath)) return configuredPath;

            // 1. Try relative to execution folder (AppContext.BaseDirectory)
            string pathInExecution = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configuredPath));
            if (File.Exists(pathInExecution)) return pathInExecution;

            // 2. Try relative to current directory (working folder)
            string pathInWorking = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), configuredPath));
            if (File.Exists(pathInWorking)) return pathInWorking;

            // 3. Fallback traversal (up to 5 levels to find Database folder structure)
            string current = AppContext.BaseDirectory;
            for (int i = 0; i < 5; i++)
            {
                if (string.IsNullOrEmpty(current)) break;
                string potential = Path.GetFullPath(Path.Combine(current, configuredPath));
                if (File.Exists(potential)) return potential;

                string? parent = Directory.GetParent(current)?.FullName;
                if (parent == current) break;
                current = parent ?? "";
            }

            return pathInWorking;
        }

        #endregion

        #region Helper Classes

        private class CsvRow
        {
            [Ignore]
            public int Index { get; set; }
            
            [Name("kanda")]
            public string Kanda { get; set; } = "";
            
            [Name("sarga")]
            public string Sarga { get; set; } = "";
            
            [Name("shloka")]
            public string Shloka { get; set; } = "";
            
            [Name("shloka_text")]
            public string ShlokaText { get; set; } = "";
            
            [Name("explanation")]
            public string Explanation { get; set; } = "";
        }

        private class ExcelExportRow
        {
            public string Kanda { get; set; } = "";
            public string Sarga { get; set; } = "";
            public string Shloka { get; set; } = "";
            public string ShlokaText { get; set; } = "";
            public string ExplanationEnglish { get; set; } = "";
            public string ExplanationHindi { get; set; } = "";
        }

        #endregion
    }
}
