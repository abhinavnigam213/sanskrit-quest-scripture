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
    public class RamayanaExcelIngester : IScriptureIngester
    {
        private readonly IIngestionRepository _repository;

        public string ScriptureCode => "Valmiki_Ramayana";

        public RamayanaExcelIngester(IIngestionRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task IngestAsync(string filePath, int scriptureId, int sourceId, int batchSize)
        {
            Console.WriteLine($"[Ramayana Ingester] Starting Excel ingestion from {filePath} (Scripture ID: {scriptureId}, Source ID: {sourceId})");
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
                TitlesJson: "{\"en\": \"Valmiki Ramayana\", \"hi\": \"वाल्मीकि रामायण\", \"sa\": \"वाल्मीकिरामायणम्\"}",
                DescriptionJson: "{\"en\": \"The ancient Sanskrit epic poem detailing the life and journey of Prince Rama, composed by the sage Valmiki.\", \"hi\": \"ऋषि वाल्मीकि द्वारा रचित प्राचीन संस्कृत महाकाव्य, जो राजकुमार राम के जीवन और यात्रा का वर्णन करता है।\"}",
                SequenceNumber: 1
            ));

            // Cache for Kanda and Sarga hierarchy IDs to avoid repeated database roundtrips
            var kandaHierarchyCache = new Dictionary<string, int>();
            var sargaHierarchyCache = new Dictionary<string, int>(); // Keyed by "Kanda_Sarga"

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

            var ramayanaTable = result.Tables[0]; // Load the first sheet
            if (ramayanaTable == null || ramayanaTable.Rows.Count == 0)
            {
                throw new InvalidOperationException("No table data found in the Excel file.");
            }

            Console.WriteLine($"[Ramayana Ingester] Found table with {ramayanaTable.Rows.Count} rows.");

            var versesList = new List<VerseEntry>();
            int totalProcessed = 0;
            int totalRows = ramayanaTable.Rows.Count;

            // Find column indices dynamically
            int kandaIdx = FindColumnIndex(ramayanaTable, "Kanda");
            int sargaIdx = FindColumnIndex(ramayanaTable, "Sarga");
            int shlokaIdx = FindColumnIndex(ramayanaTable, "Shloka");
            int shlokaTextIdx = FindColumnIndex(ramayanaTable, "ShlokaText");
            int expEnIdx = FindColumnIndex(ramayanaTable, "ExplanationEnglish");
            int expHiIdx = FindColumnIndex(ramayanaTable, "ExplanationHindi");

            foreach (DataRow row in ramayanaTable.Rows)
            {
                try
                {
                    string kandaName = row[kandaIdx]?.ToString()?.Trim() ?? "";
                    string sargaName = row[sargaIdx]?.ToString()?.Trim() ?? "";
                    string shlokaNum = row[shlokaIdx]?.ToString()?.Trim() ?? "";
                    string shlokaText = row[shlokaTextIdx]?.ToString()?.Trim() ?? "";
                    string? explanationEn = row[expEnIdx]?.ToString()?.Trim();
                    string? explanationHi = row[expHiIdx]?.ToString()?.Trim();

                    if (string.IsNullOrWhiteSpace(explanationEn)) explanationEn = null;
                    if (string.IsNullOrWhiteSpace(explanationHi)) explanationHi = null;

                    if (string.IsNullOrWhiteSpace(kandaName) || string.IsNullOrWhiteSpace(sargaName))
                        continue;

                    // Resolve Kanda Hierarchy Node
                    if (!kandaHierarchyCache.TryGetValue(kandaName, out int kandaNodeId))
                    {
                        var kandaMeta = GetKandaMetadata(kandaName);
                        string kandaLabel = kandaName.Replace(" ", "_").Replace("-", "_");
                        
                        kandaNodeId = await _repository.EnsureHierarchyNodeAsync(new HierarchyNode(
                            ScriptureId: scriptureId,
                            ParentId: rootHierarchyId,
                            LocalLabel: kandaLabel,
                            NodeType: "Kanda",
                            TitlesJson: JsonSerializer.Serialize(new Dictionary<string, string>
                            {
                                { "en", kandaName },
                                { "hi", kandaMeta.Hi },
                                { "sa", kandaMeta.Sa }
                            }),
                            DescriptionJson: JsonSerializer.Serialize(new Dictionary<string, string>
                            {
                                { "en", $"The {kandaName} section of Ramayana" },
                                { "hi", $"रामायण का {kandaMeta.Hi}" }
                            }),
                            SequenceNumber: GetKandaSequenceNumber(kandaName)
                        ));
                        kandaHierarchyCache[kandaName] = kandaNodeId;
                        Console.WriteLine($"[Ramayana Ingester] Resolved hierarchy node for Kanda: {kandaName}");
                    }

                    // Resolve Sarga Hierarchy Node
                    string sargaCacheKey = $"{kandaName}_{sargaName}";
                    if (!sargaHierarchyCache.TryGetValue(sargaCacheKey, out int sargaNodeId))
                    {
                        string sargaLabel = $"Sarga_{sargaName.Replace(" ", "_").Replace("-", "_")}";
                        
                        int sargaSeq = int.TryParse(sargaName, out int num) ? num : 0;
                        sargaNodeId = await _repository.EnsureHierarchyNodeAsync(new HierarchyNode(
                            ScriptureId: scriptureId,
                            ParentId: kandaNodeId,
                            LocalLabel: sargaLabel,
                            NodeType: "Sarga",
                            TitlesJson: JsonSerializer.Serialize(new Dictionary<string, string>
                            {
                                { "en", $"Sarga {sargaName}" },
                                { "hi", $"सर्ग {sargaName}" },
                                { "sa", $"सर्गः {sargaName}" }
                            }),
                            DescriptionJson: JsonSerializer.Serialize(new Dictionary<string, string>
                            {
                                { "en", $"Sarga {sargaName} in {kandaName}" },
                                { "hi", $"{kandaName} के अंतर्गत सर्ग {sargaName}" }
                            }),
                            SequenceNumber: sargaSeq
                        ));
                        sargaHierarchyCache[sargaCacheKey] = sargaNodeId;
                    }

                    // Verse Data JSON
                    var verseData = new Dictionary<string, object?>
                    {
                        { "translation_en", explanationEn },
                        { "translation_hi", explanationHi },
                        { "word_by_word_breakdown", Array.Empty<object>() }
                    };
                    string verseDataJson = JsonSerializer.Serialize(verseData);

                    // Structured verse number, e.g., "1.1.1" (Kanda index, Sarga, Shloka)
                    string verseNumber = $"{kandaHierarchyCache.Count}.{sargaName}.{shlokaNum}";

                    versesList.Add(new VerseEntry(
                        HierarchyId: sargaNodeId,
                        VerseNumber: verseNumber,
                        VerseType: "Shloka",
                        ContentSanskrit: shlokaText,
                        VerseDataJson: verseDataJson,
                        SourceId: sourceId,
                        SearchWeight: 5,
                        MetaTagsJson: "[]"
                    ));

                    // Ingest in batches
                    if (versesList.Count >= batchSize)
                    {
                        await _repository.IngestVersesBatchAsync(versesList);
                        totalProcessed += versesList.Count;
                        versesList.Clear();
                        Console.WriteLine($"[Ramayana Ingester] Processed {totalProcessed} / {totalRows} rows ({(double)totalProcessed / totalRows * 100:F1}%). Remaining: {totalRows - totalProcessed}");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[Ramayana Ingester] Error processing row at index {totalProcessed + versesList.Count + 1}: {ex.Message}");
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

            Console.WriteLine($"[Ramayana Ingester] Finished ingestion successfully! Processed {totalProcessed} verses.");
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

        private (string Hi, string Sa) GetKandaMetadata(string kandaName)
        {
            return kandaName.ToLower() switch
            {
                var k when k.Contains("bala") => ("बालकाण्ड", "बालकाण्डम्"),
                var k when k.Contains("ayodhya") => ("अयोध्याकाण्ड", "अयोध्याकाण्डम्"),
                var k when k.Contains("aranya") => ("अरण्यकाण्ड", "अरण्यकाण्डम्"),
                var k when k.Contains("kishkindha") => ("किष्किन्धाकाण्ड", "किष्किन्धाकाण्डम्"),
                var k when k.Contains("sundara") => ("सुन्दरकाण्ड", "सुन्दरकाण्डम्"),
                var k when k.Contains("yuddha") || k.Contains("lanka") => ("युद्धकाण्ड", "युद्धकाण्डम्"),
                var k when k.Contains("uttara") => ("उत्तरकाण्ड", "उत्तरकाण्डम्"),
                _ => ("काण्ड", "काण्डम्")
            };
        }

        private int GetKandaSequenceNumber(string kandaName)
        {
            return kandaName.ToLower() switch
            {
                var k when k.Contains("bala") => 1,
                var k when k.Contains("ayodhya") => 2,
                var k when k.Contains("aranya") => 3,
                var k when k.Contains("kishkindha") => 4,
                var k when k.Contains("sundara") => 5,
                var k when k.Contains("yuddha") || k.Contains("lanka") => 6,
                var k when k.Contains("uttara") => 7,
                _ => 99
            };
        }
    }
}
