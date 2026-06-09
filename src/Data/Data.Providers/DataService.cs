using SanskritQuest.Data.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SanskritQuest.Data.Providers;

public class DataService
{
    private readonly List<Scripture> _popularScriptures = new();
    private readonly Dictionary<string, DictionaryEntry> _specializedDictionary = new();
    private readonly Dictionary<string, GenericWordDetails> _commonDictionary = new();
    private readonly Dictionary<string, ScriptureAnalyzeResponse> _popularArchive = new();

    public List<Scripture> PopularScriptures => _popularScriptures;
    public Dictionary<string, DictionaryEntry> SpecializedDictionary => _specializedDictionary;
    public Dictionary<string, GenericWordDetails> CommonDictionary => _commonDictionary;
    public Dictionary<string, ScriptureAnalyzeResponse> PopularArchive => _popularArchive;

    public Dictionary<string, DictionaryEntry> VedasDict { get; private set; } = new();
    public Dictionary<string, DictionaryEntry> UpanishadsDict { get; private set; } = new();
    public Dictionary<string, DictionaryEntry> GitaDict { get; private set; } = new();
    public Dictionary<string, DictionaryEntry> RamayanaDict { get; private set; } = new();
    public Dictionary<string, DictionaryEntry> PuranasDict { get; private set; } = new();

    public DataService()
    {
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            string dataDir = FindDataDirectory();
            Console.WriteLine($"[DataService] Resolved data directory to: {dataDir}");

            // Load Popular Scriptures
            string scripturesPath = Path.Combine(dataDir, "popular_scriptures.json");
            if (File.Exists(scripturesPath))
            {
                var content = File.ReadAllText(scripturesPath);
                var items = JsonSerializer.Deserialize<List<Scripture>>(content);
                if (items != null) _popularScriptures.AddRange(items);
            }

            // Load Specialized Dictionary
            string specDictPath = Path.Combine(dataDir, "specialized_dictionary.json");
            if (File.Exists(specDictPath))
            {
                var content = File.ReadAllText(specDictPath);
                var items = JsonSerializer.Deserialize<Dictionary<string, DictionaryEntry>>(content);
                if (items != null)
                {
                    foreach (var (key, val) in items)
                    {
                        _specializedDictionary[key] = val;
                    }
                }
            }

            // Load Common Dictionary
            string commonDictPath = Path.Combine(dataDir, "common_dictionary.json");
            if (File.Exists(commonDictPath))
            {
                var content = File.ReadAllText(commonDictPath);
                var items = JsonSerializer.Deserialize<Dictionary<string, GenericWordDetails>>(content);
                if (items != null)
                {
                    foreach (var (key, val) in items)
                    {
                        _commonDictionary[key] = val;
                    }
                }
            }

            // Load Popular Archive
            string archivePath = Path.Combine(dataDir, "popular_archive.json");
            if (File.Exists(archivePath))
            {
                var content = File.ReadAllText(archivePath);
                var items = JsonSerializer.Deserialize<Dictionary<string, ScriptureAnalyzeResponse>>(content);
                if (items != null)
                {
                    foreach (var (key, val) in items)
                    {
                        _popularArchive[key] = val;
                    }
                }
            }

            // Partition the dict maps
            VedasDict = _specializedDictionary.Where(kv => kv.Value.Category == "Vedas").ToDictionary(kv => kv.Key, kv => kv.Value);
            UpanishadsDict = _specializedDictionary.Where(kv => kv.Value.Category == "Upanishads").ToDictionary(kv => kv.Key, kv => kv.Value);
            GitaDict = _specializedDictionary.Where(kv => kv.Value.Category == "Gita").ToDictionary(kv => kv.Key, kv => kv.Value);
            RamayanaDict = _specializedDictionary.Where(kv => kv.Value.Category == "Ramayana").ToDictionary(kv => kv.Key, kv => kv.Value);
            PuranasDict = _specializedDictionary.Where(kv => kv.Value.Category == "Puranas").ToDictionary(kv => kv.Key, kv => kv.Value);

            Console.WriteLine($"Loaded: {PopularScriptures.Count} popular scriptures, {SpecializedDictionary.Count} specialized dictionary words, {CommonDictionary.Count} common words, {PopularArchive.Count} archived verses.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DataService] CRITICAL Error loading JSON datasets: {ex.Message}");
        }
    }

    private string FindDataDirectory()
    {
        // 1. Production/Publish directory check
        string publishedPath = Path.Combine(AppContext.BaseDirectory, "DataFiles");
        if (Directory.Exists(publishedPath) && File.Exists(Path.Combine(publishedPath, "popular_scriptures.json")))
        {
            return publishedPath;
        }

        // 2. Development directory check (running via dotnet run from Web.Api/ or project root)
        string localDevPath = Path.Combine(Directory.GetCurrentDirectory(), "DataFiles");
        if (Directory.Exists(localDevPath) && File.Exists(Path.Combine(localDevPath, "popular_scriptures.json")))
        {
            return Path.GetFullPath(localDevPath);
        }

        // 3. Fallback traversal check
        string current = AppContext.BaseDirectory;
        while (!string.IsNullOrEmpty(current))
        {
            string potential = Path.Combine(current, "DataFiles");
            if (Directory.Exists(potential) && File.Exists(Path.Combine(potential, "popular_scriptures.json")))
            {
                return potential;
            }
            string? parent = Directory.GetParent(current)?.FullName;
            if (parent == current) break;
            current = parent ?? string.Empty;
        }

        return "./DataFiles";
    }
}
