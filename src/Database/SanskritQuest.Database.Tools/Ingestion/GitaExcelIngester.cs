using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ExcelDataReader;
using SanskritQuest.Database.Tools.Repository;

namespace SanskritQuest.Database.Tools.Ingestion
{
    public class GitaExcelIngester : IScriptureIngester
    {
        private readonly IIngestionRepository _repository;

        public string ScriptureCode => "Bhagavad_Gita";

        public GitaExcelIngester(IIngestionRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task IngestAsync(string filePath, int scriptureId, int sourceId, int batchSize)
        {
            Console.WriteLine($"[Gita Ingester] Starting ingestion of {filePath} (Scripture ID: {scriptureId}, Source ID: {sourceId})");
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Excel file not found at path: {filePath}");
            }

            // 1. Ensure Root Hierarchy Node exists
            int rootHierarchyId = await _repository.EnsureHierarchyNodeAsync(new HierarchyNode(
                ScriptureId: scriptureId,
                ParentId: null,
                LocalLabel: ScriptureCode,
                NodeType: "Scripture",
                TitlesJson: "{\"en\": \"Bhagavad Gita\", \"hi\": \"भगवद्गीता\", \"sa\": \"श्रीमद्भगवद्गीता\"}",
                DescriptionJson: "{\"en\": \"The dialogue between Sri Krishna and Arjuna on duty and duty-less action.\", \"hi\": \"कर्तव्य और निष्काम कर्म पर श्री कृष्ण और अर्जुन के बीच संवाद।\"}",
                SequenceNumber: 1
            ));

            // 2. Open and read spreadsheet
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });

            var gitaTable = result.Tables["Bhagavad-Gita"];
            if (gitaTable == null)
            {
                throw new InvalidOperationException("Sheet 'Bhagavad-Gita' not found in Excel workbook.");
            }

            Console.WriteLine($"[Gita Ingester] Found sheet 'Bhagavad-Gita' with {gitaTable.Rows.Count} rows.");

            // 3. Pre-create/resolve all chapter nodes
            var chapterHierarchyIds = new Dictionary<int, int>();
            for (int i = 1; i <= 18; i++)
            {
                var names = GetChapterNames(i);
                int chapterNodeId = await _repository.EnsureHierarchyNodeAsync(new HierarchyNode(
                    ScriptureId: scriptureId,
                    ParentId: rootHierarchyId,
                    LocalLabel: $"Adhyaya_{i}",
                    NodeType: "Chapter",
                    TitlesJson: JsonSerializer.Serialize(new Dictionary<string, string>
                    {
                        { "en", $"Chapter {i}: {names.En}" },
                        { "hi", $"अध्याय {i}: {names.Hi}" },
                        { "sa", names.Sa }
                    }),
                    DescriptionJson: JsonSerializer.Serialize(new Dictionary<string, string>
                    {
                        { "en", $"The yoga of {names.En.ToLower()}" },
                        { "hi", $"{names.Hi} का योग" }
                    }),
                    SequenceNumber: i
                ));
                chapterHierarchyIds[i] = chapterNodeId;
            }

            // 4. Process row by row in batches
            var versesList = new List<VerseEntry>();
            int totalProcessed = 0;
            int totalRows = gitaTable.Rows.Count;

            // Find column indices dynamically
            int sNoIdx = FindColumnIndex(gitaTable, "S.No.");
            int titleIdx = FindColumnIndex(gitaTable, "Title");
            int chapterIdx = FindColumnIndex(gitaTable, "Chapter");
            int verseIdx = FindColumnIndex(gitaTable, "Verse");
            int sanskritIdx = FindColumnIndex(gitaTable, "Sanskrit Anuvad");
            int hindiIdx = FindColumnIndex(gitaTable, "Hindi Anuvad");
            int englishIdx = FindColumnIndex(gitaTable, "Enlgish Translation"); // Note the spreadsheet typo

            foreach (DataRow row in gitaTable.Rows)
            {
                try
                {
                    string chapterStr = row[chapterIdx]?.ToString() ?? "";
                    string verseStr = row[verseIdx]?.ToString() ?? "";
                    string sanskritText = row[sanskritIdx]?.ToString() ?? "";
                    string? hindiText = row[hindiIdx]?.ToString();
                    hindiText = string.IsNullOrWhiteSpace(hindiText) ? null : hindiText.Trim();
                    string? englishText = row[englishIdx]?.ToString();
                    englishText = string.IsNullOrWhiteSpace(englishText) ? null : englishText.Trim();

                    if (string.IsNullOrWhiteSpace(chapterStr) || string.IsNullOrWhiteSpace(verseStr))
                        continue;

                    // Parse chapter number
                    int chapterNum = ParseChapterNumber(chapterStr);
                    if (chapterNum < 1 || chapterNum > 18) continue;

                    int chapterHierarchyId = chapterHierarchyIds[chapterNum];

                    // Clean verse number (e.g., "Verse 1.1" -> "1.1")
                    string verseNumber = verseStr.Replace("Verse", "", StringComparison.OrdinalIgnoreCase).Trim();

                    // Localized Verse Data JSON
                    var verseData = new Dictionary<string, object?>
                    {
                        { "translation_en", englishText },
                        { "translation_hi", hindiText },
                        { "word_by_word_breakdown", Array.Empty<object>() }
                    };
                    string verseDataJson = JsonSerializer.Serialize(verseData);

                    versesList.Add(new VerseEntry(
                        HierarchyId: chapterHierarchyId,
                        VerseNumber: verseNumber,
                        VerseType: "Shloka",
                        ContentSanskrit: sanskritText,
                        VerseDataJson: verseDataJson,
                        SourceId: sourceId,
                        SearchWeight: 5,
                        MetaTagsJson: "[]"
                    ));

                    // Ingest batch if threshold met
                    if (versesList.Count >= batchSize)
                    {
                        await _repository.IngestVersesBatchAsync(versesList);
                        totalProcessed += versesList.Count;
                        versesList.Clear();
                        Console.WriteLine($"[Gita Ingester] Processed {totalProcessed} / {totalRows} rows ({(double)totalProcessed / totalRows * 100:F1}%). Remaining: {totalRows - totalProcessed}");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[Gita Ingester] Error processing row at index {totalProcessed + versesList.Count + 1}: {ex.Message}");
                    Console.ResetColor();
                }
            }

            // Ingest remaining
            if (versesList.Count > 0)
            {
                await _repository.IngestVersesBatchAsync(versesList);
                totalProcessed += versesList.Count;
                versesList.Clear();
            }

            Console.WriteLine($"[Gita Ingester] Finished ingestion successfully! Processed {totalProcessed} verses.");
        }

        private int FindColumnIndex(DataTable table, string colName)
        {
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (table.Columns[i].ColumnName.Trim().Equals(colName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            throw new ArgumentException($"Required column '{colName}' not found in sheet. Available columns: {string.Join(", ", table.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}");
        }

        private int ParseChapterNumber(string chapterStr)
        {
            // "Chapter 1" -> 1
            var cleanStr = chapterStr.Replace("Chapter", "", StringComparison.OrdinalIgnoreCase).Trim();
            if (int.TryParse(cleanStr, out int num))
            {
                return num;
            }
            return -1;
        }

        private (string En, string Hi, string Sa) GetChapterNames(int chapterNum)
        {
            return chapterNum switch
            {
                1 => ("Arjuna's Vishada Yoga", "अर्जुन विषाद योग", "अर्जुनविषादयोगः"),
                2 => ("Sankhya Yoga", "सांख्य योग", "सांख्ययोगः"),
                3 => ("Karma Yoga", "कर्म योग", "कर्मयोगः"),
                4 => ("Jnana Karma Sanyasa Yoga", "ज्ञान कर्म संन्यास योग", "ज्ञानकर्मसंन्यासयोगः"),
                5 => ("Karma Sanyasa Yoga", "कर्म संन्यास योग", "कर्मसंन्यासयोगः"),
                6 => ("Atma Samyama Yoga", "आत्मसंयम योग", "आत्मसंयमयोगः"),
                7 => ("Jnana Vijnana Yoga", "ज्ञान विज्ञान योग", "ज्ञानविज्ञानयोगः"),
                8 => ("Aksara Brahma Yoga", "अक्षर ब्रह्म योग", "अक्षरब्रह्मयोगः"),
                9 => ("Raja Vidya Raja Guhya Yoga", "राजविद्या राजगुह्य योग", "राजविद्याराजगुह्ययोगः"),
                10 => ("Vibhuti Yoga", "विभूति योग", "विभूतियोगः"),
                11 => ("Viswarupa Darsana Yoga", "विश्वरूप दर्शन योग", "विश्वरूपदर्शनयोगः"),
                12 => ("Bhakti Yoga", "भक्ति योग", "भक्तियोगः"),
                13 => ("Ksetra Ksetrajna Vibhaga Yoga", "क्षेत्र क्षेत्रज्ञ विभाग योग", "क्षेत्रक्षेत्रज्ञविभागयोगः"),
                14 => ("Gunatraya Vibhaga Yoga", "गुणत्रय विभाग योग", "गुणत्रयविभागयोगः"),
                15 => ("Purusottama Yoga", "पुरुषोत्तम योग", "पुरुषोत्तमयोगः"),
                16 => ("Daivasura Sampad Vibhaga Yoga", "दैवासुर सम्पद विभाग योग", "दैवासुरसम्पद्विभागयोगः"),
                17 => ("Shraddhatraya Vibhaga Yoga", "श्रद्धात्रय विभाग योग", "श्रद्धात्रयविभागयोगः"),
                18 => ("Moksha Sanyasa Yoga", "मोक्ष संन्यास योग", "मोक्षसंन्यासयोगः"),
                _ => ("Unknown", "अज्ञात", "अज्ञात")
            };
        }
    }
}
