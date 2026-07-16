using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SanskritQuest.Database.Tools.Repository;

namespace SanskritQuest.Database.Tools.Ingestion
{
    public class ScriptureConfig
    {
        public string FilePath { get; set; } = "";
        public string SourceName { get; set; } = "";
        public string SourceUrl { get; set; } = "";
        public int ScriptureId { get; set; }
        public int SourceId { get; set; }
    }

    public class IngestionSettings
    {
        public int BatchSize { get; set; } = 500;
        public Dictionary<string, ScriptureConfig> Scriptures { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }

    public class IngestionOrchestrator
    {
        private readonly IIngestionRepository _repository;
        private readonly IEnumerable<IScriptureIngester> _ingesters;
        private readonly IngestionSettings _settings;

        public IngestionOrchestrator(
            IIngestionRepository repository, 
            IEnumerable<IScriptureIngester> ingesters,
            IConfiguration configuration)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _ingesters = ingesters ?? throw new ArgumentNullException(nameof(ingesters));
            
            _settings = configuration.GetSection("IngestionSettings").Get<IngestionSettings>() 
                ?? new IngestionSettings();
        }

        public async Task IngestScriptureAsync(string requestedScripture)
        {
            if (requestedScripture.Trim().Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("[Orchestrator] Running ingestion for all configured scriptures...");
                foreach (var scriptureKey in _settings.Scriptures.Keys)
                {
                    await IngestScriptureAsync(scriptureKey);
                }
                return;
            }

            // Normalize scripture code from requested name (e.g. "Gita" -> "Bhagavad_Gita", "Ramayana" -> "Valmiki_Ramayana")
            string scriptureCode = ResolveScriptureCode(requestedScripture);
            
            if (string.IsNullOrEmpty(scriptureCode) || !_settings.Scriptures.TryGetValue(scriptureCode, out var scriptureConfig))
            {
                throw new ArgumentException($"Ingestion configuration not found for scripture: '{requestedScripture}'. Available configurations: {string.Join(", ", _settings.Scriptures.Keys)}");
            }

            // Find matching ingester
            var ingester = _ingesters.FirstOrDefault(i => i.ScriptureCode.Equals(scriptureCode, StringComparison.OrdinalIgnoreCase));
            if (ingester == null)
            {
                throw new InvalidOperationException($"No ingester registered for scripture code: '{scriptureCode}'");
            }

            // Resolve file path dynamically
            string resolvedPath = ResolveFilePath(scriptureConfig.FilePath);
            if (!File.Exists(resolvedPath))
            {
                throw new FileNotFoundException($"Could not locate scripture file at path: {resolvedPath} (configured as {scriptureConfig.FilePath})");
            }

            Console.WriteLine($"[Orchestrator] Starting ingestion of '{scriptureCode}' (Scripture ID: {scriptureConfig.ScriptureId}, Source ID: {scriptureConfig.SourceId}) using file: {resolvedPath}");

            // Trigger the Ingestion
            var startTime = DateTime.UtcNow;
            await ingester.IngestAsync(resolvedPath, scriptureConfig.ScriptureId, scriptureConfig.SourceId, _settings.BatchSize);
            var duration = DateTime.UtcNow - startTime;

            Console.WriteLine($"[Orchestrator] Ingestion of '{scriptureCode}' completed in {duration.TotalSeconds:F2} seconds.");
        }

        private string ResolveScriptureCode(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            var normalized = input.Trim().ToLowerInvariant();

            if (normalized.Contains("gita") || normalized == "geeta" || normalized == "bg")
            {
                return "Bhagavad_Gita";
            }
            if (normalized.Contains("ramayan") || normalized == "vr")
            {
                return "Valmiki_Ramayana";
            }

            return input; // fallback to verbatim
        }

        private string ResolveFilePath(string configuredPath)
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
    }
}
