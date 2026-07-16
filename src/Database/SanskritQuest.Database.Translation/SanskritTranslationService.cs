using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace SanskritQuest.Database.Translation
{
    public class SanskritTranslationService : ISanskritTranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly TranslationOptions _options;

        public SanskritTranslationService(HttpClient httpClient, TranslationOptions options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<TranslationResult?> TranslateShlokaAsync(
            int index,
            string sanskritText,
            string? existingEnglish = null,
            CancellationToken cancellationToken = default,
            TranslationTarget? target = null)
        {
            var singleBatch = new List<(int index, string sanskrit, string? english)>
            {
                (index, sanskritText, existingEnglish)
            };

            var results = await TranslateShlokasBatchAsync(singleBatch, cancellationToken, target);
            return results != null && results.Count > 0 ? results[0] : null;
        }

        public async Task<List<TranslationResult>?> TranslateShlokasBatchAsync(
            List<(int index, string sanskrit, string? english)> batch,
            CancellationToken cancellationToken = default,
            TranslationTarget? target = null)
        {
            if (batch == null || batch.Count == 0) return new List<TranslationResult>();

            // Resolve target language (override or fallback)
            var activeTarget = target ?? _options.Target;

            // Generate Schema based on target language choice
            object schema = GetResponseSchema(activeTarget);

            // Build Prompt content
            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine("Translate the following Sanskrit shlokas from Valmiki Ramayana into target language(s) as configured by schema.");
            promptBuilder.AppendLine("Provide the response strictly as a JSON array matching the schema.");
            promptBuilder.AppendLine("Items to translate:\n");

            foreach (var item in batch)
            {
                promptBuilder.AppendLine($"Index: {item.index}");
                promptBuilder.AppendLine($"Sanskrit Shloka: {item.sanskrit}");
                if (!string.IsNullOrEmpty(item.english))
                {
                    promptBuilder.AppendLine($"Existing English Translation: {item.english}");
                }
                promptBuilder.AppendLine("---");
            }

            string systemInstruction = activeTarget switch
            {
                TranslationTarget.English => "You are an expert scriptural Indologist. Translate Sanskrit shlokas accurately into clear, theological English.",
                TranslationTarget.Hindi => "You are an expert scriptural Indologist. Translate Sanskrit shlokas and their English translations into natural, scriptural Hindi.",
                _ => "You are an expert scriptural Indologist. Translate Sanskrit shlokas into clear theological English and natural scriptural Hindi."
            };

            // Gemini API JSON Body
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = promptBuilder.ToString() }
                        }
                    }
                },
                generationConfig = new
                {
                    responseMimeType = "application/json",
                    responseSchema = schema,
                    temperature = _options.Temperature
                },
                systemInstruction = new
                {
                    parts = new[]
                    {
                        new { text = systemInstruction }
                    }
                }
            };

            string jsonPayload = JsonSerializer.Serialize(requestBody);
            string url = $"{_options.ApiEndpointUrl}/{_options.ModelName}:generateContent?key={_options.GeminiApiKey}";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Gemini API Request failed with status {response.StatusCode}: {errorContent}");
            }

            string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            return ParseGeminiResponse(responseJson);
        }

        private object GetResponseSchema(TranslationTarget activeTarget)
        {
            var properties = new Dictionary<string, object>
            {
                { "index", new { type = "INTEGER", description = "The index matching the input item." } }
            };

            var required = new List<string> { "index" };

            if (activeTarget == TranslationTarget.English || activeTarget == TranslationTarget.Both)
            {
                properties.Add("translation_en", new { type = "STRING", description = "The English translation/explanation." });
                required.Add("translation_en");
            }

            if (activeTarget == TranslationTarget.Hindi || activeTarget == TranslationTarget.Both)
            {
                properties.Add("translation_hi", new { type = "STRING", description = "The Hindi translation/explanation." });
                required.Add("translation_hi");
            }

            return new
            {
                type = "ARRAY",
                items = new
                {
                    type = "OBJECT",
                    properties = properties,
                    required = required.ToArray()
                }
            };
        }

        private List<TranslationResult>? ParseGeminiResponse(string responseJson)
        {
            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            if (!root.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
            {
                return null;
            }

            var firstCandidate = candidates[0];
            if (!firstCandidate.TryGetProperty("content", out var content) ||
                !content.TryGetProperty("parts", out var parts) || parts.GetArrayLength() == 0)
            {
                return null;
            }

            string? text = parts[0].GetProperty("text").GetString();
            if (string.IsNullOrEmpty(text)) return null;

            // Deserialize the array of translated items
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apiResults = JsonSerializer.Deserialize<List<ApiTranslationItem>>(text, options);

            if (apiResults == null) return null;

            var results = new List<TranslationResult>();
            foreach (var item in apiResults)
            {
                results.Add(new TranslationResult
                {
                    Index = item.Index,
                    TranslationEn = item.TranslationEn,
                    TranslationHi = item.TranslationHi
                });
            }

            return results;
        }

        // Inner class to match dynamic properties returned by API safely
        private class ApiTranslationItem
        {
            [JsonPropertyName("index")]
            public int Index { get; set; }

            [JsonPropertyName("translation_en")]
            public string? TranslationEn { get; set; }

            [JsonPropertyName("translation_hi")]
            public string? TranslationHi { get; set; }
        }
    }
}
