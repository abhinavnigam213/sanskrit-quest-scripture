using Microsoft.Extensions.AI;
using SanskritQuest.Business.Contracts;
using SanskritQuest.Common.Utilities;
using SanskritQuest.Data.Contracts;
using SanskritQuest.Data.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SanskritQuest.Services.AIService;

public class AIService
{
    private readonly IChatClient? _chatClient;
    private readonly DataService _dataService;

    public AIService(IChatClient? chatClient, DataService dataService)
    {
        _chatClient = chatClient;
        _dataService = dataService;
    }

    private string GetDictionaryHints(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        var hints = new List<string>();
        var words = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var w in words)
        {
            var clean = Regex.Replace(w, @"[।॥,.\n\r]", "").Trim();
            if (string.IsNullOrEmpty(clean)) continue;

            if (_dataService.SpecializedDictionary.TryGetValue(clean, out var match))
            {
                hints.Add($"- \"{clean}\": [{match.Category}] Grammar: {match.Grammar}, English definition: \"{match.Eng}\", Hindi term: \"{match.Hin}\"");
            }
        }

        if (hints.Count > 0)
        {
            return $"\nSpecialized Lexicon Hints (Use these exact theological definitions for terms from the text if they are relevant to your scriptural translation or breakdown):\n{string.Join("\n", hints)}\n";
        }
        return string.Empty;
    }

    private WordBreakdownItem? ParseGenericWordBreakdown(string word)
    {
        var cleanWord = Regex.Replace(word, @"[।॥,.\n\r]", "").Trim();
        if (string.IsNullOrEmpty(cleanWord)) return null;

        string iastWord = Transliterator.DevanagariToIast(cleanWord);

        if (_dataService.SpecializedDictionary.TryGetValue(cleanWord, out var entry))
        {
            return new WordBreakdownItem(
                Word: cleanWord,
                Grammar: $"[{entry.Category}] {entry.Grammar}",
                MeaningEnglish: entry.Eng,
                MeaningHindi: entry.Hin
            );
        }

        if (_dataService.CommonDictionary.TryGetValue(cleanWord, out var commonEntry))
        {
            return new WordBreakdownItem(
                Word: cleanWord,
                Grammar: commonEntry.Grammar,
                MeaningEnglish: commonEntry.Eng,
                MeaningHindi: commonEntry.Hin
            );
        }

        string grammar = "Sanskrit Particle / Verb / Noun";
        string meaningEnglish = $"Transliterated: {iastWord}";
        string meaningHindi = $"लिप्यन्तरण: {iastWord}";

        if (cleanWord.EndsWith("स्य"))
        {
            grammar = "Genitive Singular Noun (Masculine/Neuter)";
            var root = cleanWord[..^2];
            var rootIast = Transliterator.DevanagariToIast(root);
            meaningEnglish = $"of / belonging to {rootIast}";
            meaningHindi = $"{root} का / के / की";
        }
        else if (cleanWord.EndsWith("ेषु") || cleanWord.EndsWith("षु"))
        {
            grammar = "Locative Plural Noun";
            var root = cleanWord.EndsWith("ेषु") ? cleanWord[..^3] : cleanWord[..^2];
            meaningEnglish = $"amongst or within the elements of {Transliterator.DevanagariToIast(root)}";
            meaningHindi = $"{(string.IsNullOrEmpty(root) ? cleanWord : root)} में / पर / सभी में";
        }
        else if (cleanWord.EndsWith("े"))
        {
            grammar = "Locative Singular / Masculine Vocative";
            meaningEnglish = $"in / at / upon {iastWord}";
            meaningHindi = $"{cleanWord} में / के भीतर";
        }
        else if (cleanWord.EndsWith("ात्"))
        {
            grammar = "Ablative Singular Noun";
            var root = cleanWord[..^3];
            meaningEnglish = $"originating from / due to {Transliterator.DevanagariToIast(root)}";
            meaningHindi = $"{(string.IsNullOrEmpty(root) ? cleanWord : root)} से / कारण से";
        }
        else if (cleanWord.EndsWith("ः"))
        {
            grammar = "Nominative Singular Noun (Masculine)";
            meaningEnglish = $"the subject: {iastWord}";
            meaningHindi = "कर्ता कारक रूप";
        }
        else if (cleanWord.EndsWith("म्"))
        {
            grammar = "Accusative Singular / Neuter Nominative";
            meaningEnglish = $"the object or container of {iastWord[..^1]}";
            meaningHindi = "कर्म कारक / तटस्थ स्वरूप";
        }
        else if (cleanWord.EndsWith("ामि"))
        {
            grammar = "Verb: Present Active 1st Person Singular";
            meaningEnglish = $"I perform / manifest action of {iastWord}";
            meaningHindi = "मैं क्रिया करता / करती हूँ";
        }
        else if (cleanWord.EndsWith("ति"))
        {
            grammar = "Verb: Present Active 3rd Person Singular";
            meaningEnglish = "performs or undergoes the act";
            meaningHindi = "वह क्रिया सम्पन्न करता है";
        }

        return new WordBreakdownItem(cleanWord, grammar, meaningHindi, meaningEnglish);
    }

    private ScriptureAnalyzeResponse? TryMatchingScriptureArchive(string text)
    {
        string normInput = Regex.Replace(text, @"[\s\r\n\t।॥.,\/#!$%\^&\*;:{}=\-_`~() ]", "").ToLower();
        if (normInput.Length < 5) return null;

        if (normInput.Contains("कर्मण्ये") || normInput.Contains("karmanyeva") ||
            (normInput.Contains("कर्म") && normInput.Contains("फलेषु")) ||
            (normInput.Contains("karma") && normInput.Contains("phaleshu")))
        {
            if (_dataService.PopularArchive.TryGetValue("karma", out var match)) return match;
        }

        if (normInput.Contains("यदा") && normInput.Contains("धर्मस्य") ||
            normInput.Contains("yada") && normInput.Contains("dharmasya") ||
            normInput.Contains("सृजाम्यहम्") || normInput.Contains("srijamyaham"))
        {
            if (_dataService.PopularArchive.TryGetValue("yada", out var match)) return match;
        }

        if (normInput.Contains("त्र्यम्बक") || normInput.Contains("tryambaka") ||
            normInput.Contains("सुगन्धि") || normInput.Contains("sugandhi") ||
            normInput.Contains("मृत्योर्मुक्षीय") || normInput.Contains("mrityormukshiya"))
        {
            if (_dataService.PopularArchive.TryGetValue("tryambakam", out var match)) return match;
        }

        if (normInput.Contains("भूर्भुव") || normInput.Contains("bhurbhuva") ||
            normInput.Contains("सवितु") || normInput.Contains("savitur") ||
            normInput.Contains("प्रचोदयात्") || normInput.Contains("prachodayat"))
        {
            if (_dataService.PopularArchive.TryGetValue("om bhur", out var match)) return match;
        }

        if (normInput.Contains("सहनाववतु") || (normInput.Contains("sahana") && normInput.Contains("navavatu")) ||
            normInput.Contains("विद्विषावहै") || normInput.Contains("vidvishavahai"))
        {
            if (_dataService.PopularArchive.TryGetValue("saha na", out var match)) return match;
        }

        if (normInput.Contains("ईशावास्य") || normInput.Contains("ishavasya") ||
            normInput.Contains("त्यक्तेन") || normInput.Contains("tyaktena") ||
            (normInput.Contains("धनम्") && normInput.Contains("कस्य")) ||
            (normInput.Contains("kasya") && normInput.Contains("dhanam")))
        {
            if (_dataService.PopularArchive.TryGetValue("isha vasyam", out var match)) return match;
        }

        return null;
    }

    public async Task<TranslationResponse> TranslateTextAsync(
        string text,
        string sourceLang,
        string targetLang,
        string? scriptureContext)
    {
        try
        {
            if (_chatClient == null)
            {
                throw new InvalidOperationException("IChatClient is not configured or setup.");
            }

            string contextPrompt = !string.IsNullOrEmpty(scriptureContext)
                ? $"The text is from {scriptureContext} or related Hindu scripture."
                : string.Empty;

            string dictHints = GetDictionaryHints(text);
            string prompt = $@"Translate the following text.
                                Source Language: {sourceLang}
                                Target Language: {targetLang}
                                Context: {contextPrompt}
                                {dictHints}

                                Text to translate:
                                ""{text}""

                                Provide the translation, a word-by-word grammar and meaning breakdown (called Padapatha parsing, which splits sandhi compounds if source contains Sanskrit, or breakdown of words if not), and a philological/philosophical scriptural explanation.";

            string systemInstruction = @"You are an expert philologist, Indologist, and scriptural scholar specializing in Sanskrit scriptures (Vedas, Upanishads, Bhagavad Gita, Puranas, Ramayana) as well as Hindi and English scriptural commentaries.
                                            Translate text precisely. When dealing with Sanskrit, respect the philosophical nuances and render exact meanings.
                                            Provide the output strictly in the specified JSON structure.";

            var options = new ChatOptions
            {
                ResponseFormat = ChatResponseFormat.Json,
            };

            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, systemInstruction),
                new(ChatRole.User, prompt)
            };

            var chatResponse = await _chatClient.CompleteAsync(messages, options);
            string responseText = chatResponse.Message.Text ?? string.Empty;

            var result = JsonSerializer.Deserialize<TranslationResponse>(responseText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null)
            {
                throw new Exception("Unable to deserialize translation model from AI response.");
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AIService] TranslateTextAsync Failure fallback active: {ex.Message}");
            return GenerateTranslateFallback(text, targetLang);
        }
    }

    private TranslationResponse GenerateTranslateFallback(string text, string targetLang)
    {
        bool isHindi = targetLang.Equals("hindi", StringComparison.OrdinalIgnoreCase);

        // Try Archive
        var archiveMatch = TryMatchingScriptureArchive(text);
        if (archiveMatch != null)
        {
            return new TranslationResponse(
                SourceLang: "sanskrit",
                TargetLang: targetLang,
                TranslatedText: isHindi ? archiveMatch.TranslationHindi : archiveMatch.TranslationEnglish,
                Explanation: $"{archiveMatch.SpiritualSignificance} (⚠️ Computed via Local Archive Backup because Live API is not configured or quota is exhausted)",
                WordBreakdown: archiveMatch.WordBreakdown,
                IsFallback: true
            );
        }

        // Token list fallback
        var tokens = text.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Take(15);
        var breakdown = tokens.Select(ParseGenericWordBreakdown).Where(b => b != null).Cast<WordBreakdownItem>().ToList();
        string iastText = Transliterator.DevanagariToIast(text);

        string cleanInput = Regex.Replace(text, @"[।॥,.\n\r]", "").Trim();
        string trText = "";

        if (_dataService.CommonDictionary.TryGetValue(cleanInput, out var dictMatch))
        {
            trText = isHindi ? dictMatch.Hin : dictMatch.Eng;
        }
        else
        {
            var words = tokens.Select(tok =>
            {
                var wordClean = Regex.Replace(tok, @"[।॥,.\n\r]", "").Trim();
                if (_dataService.CommonDictionary.TryGetValue(wordClean, out var m))
                {
                    return isHindi ? m.Hin : m.Eng;
                }
                return null;
            }).Where(w => w != null).ToList();

            if (words.Count > 0)
            {
                trText = string.Join(" / ", words);
            }
            else
            {
                trText = isHindi ? $"हिन्दी रूप: \"{text}\"" : $"Rendered translation: \"{iastText}\"";
            }
        }

        return new TranslationResponse(
            SourceLang: "sanskrit",
            TargetLang: targetLang,
            TranslatedText: trText,
            Explanation: $"⚠️ [VedicEngine Offline Backup Active] Live AI translation services are currently unavailable or rate-limited. Character sandhis and dictionaries mapped locally.\n\nRaw text: \"{iastText}\"",
            WordBreakdown: breakdown,
            IsFallback: true
        );
    }

    public async Task<TransliterateResponse> TransliterateTextAsync(
        string text,
        string sourceScript,
        string targetScript)
    {
        try
        {
            if (_chatClient == null)
            {
                throw new InvalidOperationException("IChatClient is not configured.");
            }

            string prompt = $@"Convert (transliterate) the following text from the source script to the target script. 
Source Script: {sourceScript}
Target Script: {targetScript}

Input Text:
{text}

For your diacritic markers (in IAST), be exceptionally precise (e.g. using dots below for retroflexes like ṣ, ṭ, ḍ, line above for long vowels like ā, ī, ū, and dot above for anusvara m, etc.). Preservation of line structures is required.";

            string systemInstruction = @"You are a helper specializing in Indic script transliteration.
Your goal is to transliterate characters purely phonetically/orthographically between Devanagari (देवानागरी), IAST (International Alphabet of Sanskrit Transliteration with diacritics), ITRANS (ASCII), SLP1 (Sanskrit Library Phonetic basic ASCII encoding standard), and English Phonetic (for chanting/singing).
Do NOT translate the meaning, only convert the phonetic representation from one system to another.
In SLP1, map retroflex consonants to w, W, q, Q, R, sibilants to S/z, nasals to N/Y/R/n/m, and vowels and anusvara according to Sanskrit Library SLP1 specs.
Provide the output strictly in JSON.";

            var options = new ChatOptions
            {
                ResponseFormat = ChatResponseFormat.Json,
            };

            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, systemInstruction),
                new ChatMessage(ChatRole.User, prompt)
            };

            var chatResponse = await _chatClient.CompleteAsync(messages, options);
            string responseText = chatResponse.Message.Text ?? string.Empty;

            var result = JsonSerializer.Deserialize<TransliterateResponse>(responseText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null)
            {
                throw new Exception("Unable to deserialize transliteration model from AI response.");
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AIService] TransliterateTextAsync Exception fallback active: {ex.Message}");
            return GenerateTransliterateFallback(text, sourceScript, targetScript);
        }
    }

    private TransliterateResponse GenerateTransliterateFallback(string text, string sourceScript, string targetScript)
    {
        string outText = text;
        if (sourceScript.Equals("devanagari", System.StringComparison.OrdinalIgnoreCase))
        {
            if (targetScript.Equals("slp1", System.StringComparison.OrdinalIgnoreCase))
            {
                outText = Transliterator.DevanagariToSlp1(text);
            }
            else
            {
                string iast = Transliterator.DevanagariToIast(text);
                if (targetScript.Equals("iast", System.StringComparison.OrdinalIgnoreCase))
                {
                    outText = iast;
                }
                else if (targetScript.Equals("english_phonetic", System.StringComparison.OrdinalIgnoreCase))
                {
                    outText = Transliterator.IastToPhonetic(iast);
                }
                else if (targetScript.Equals("itrans", System.StringComparison.OrdinalIgnoreCase))
                {
                    outText = Transliterator.IastToItrans(iast);
                }
            }
        }
        else
        {
            outText = $"[Offline Transliteration] {text}";
        }

        return new TransliterateResponse(sourceScript, targetScript, outText);
    }

    public async Task<ScriptureAnalyzeResponse> AnalyzeScriptureAsync(
        string text,
        string? sourceContext)
    {
        try
        {
            if (_chatClient == null)
            {
                throw new InvalidOperationException("IChatClient is not configured.");
            }

            string contextText = !string.IsNullOrEmpty(sourceContext)
                ? $"Contextual source hint: {sourceContext}"
                : string.Empty;

            string dictHints = GetDictionaryHints(text);
            string prompt = $@"Analyze the following Hindu scripture/verse in deep detail:
""{text}""
{contextText}
{dictHints}

Extract or compute:
1. Identified scripture source (e.g. Bhagavad Gita chapter/verse, Veda mandala, Upanishad name).
2. Clean IAST transliteration (with correct diacritics).
3. Phonetic English transliteration (friendly for active chanting).
4. English prose translation.
5. Hindi prose translation.
6. Spiritual/Theological significance & commentary.
7. Word-by-word grammatical breakdown (declensions, root words, nouns/verbs, individual meanings).
8. Poetic Meter name if applicable (e.g., Anustubh, Gayatri, Tristubh, Jagati etc.)";

            string systemInstruction = @"You are a high-level Sanatana Dharma and Sanskrit scripture scholar.
Analyze the provided Sanskrit/Hindi/English scripture.

CRITICAL WORD VS VERSE CONSTRAINTS:
- If the input text is a single word, a small compound, or a short theological/philosophical term (typically 1 to 3 words, e.g., 'योगः', 'ज्ञानम्', 'कर्म', 'आत्मनः') rather than an actual complete scripture verse or a clear multi-word verse fragment:
  1. Do NOT try to reconstruct a completely different verse or return translations/commentary of an unrelated longer scripture (for example, do NOT reconstruct and return BG 2.48 'samatvaṁ yoga ucyate...' if the user only entered the single term 'योगः').
  2. Keep the ""verse"" field equal to the input word itself (or its clean Devanagari form if transliterated).
  3. Treat the input as a single theological/intellectual concept or keyword. Refer to its context/source if provided (e.g. 'Gita' or 'Vedas'), identifying it as a 'Scriptural Concept' or 'Key Term' in the ""identifiedSource"" field.
  4. Translate the specific concept/word directly and precisely into English and Hindi (""translationEnglish"" and ""translationHindi"" fields).
  5. In the ""spiritualSignificance"" field, provide a rich, deep theological explanation of this specific concept, quoting or citing how it is used across the scripture(s), rather than pretending the word itself is a full verse.
  6. In the ""wordBreakdown"" array, include exactly the breakdown of this single word (its root, conjugation/declension, prefix/suffix, meaning).
  7. Set the ""poeticMeter"" to ""None / N/A"".

- However, if the input is a genuine verse fragment or sloka lines (e.g., 'कर्मण्येवाधिकारस्ते...'):
  1. Reconstruct and analyze the complete verse as expected.
  2. Provide proper translations, spiritual significance, and full word-by-word breakdown for the whole verse.
  
Always provide the response conforming strictly to the requested JSON structure.";

            var options = new ChatOptions
            {
                ResponseFormat = ChatResponseFormat.Json,
            };

            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, systemInstruction),
                new ChatMessage(ChatRole.User, prompt)
            };

            var chatResponse = await _chatClient.CompleteAsync(messages, options);
            string responseText = chatResponse.Message.Text ?? string.Empty;

            var result = JsonSerializer.Deserialize<ScriptureAnalyzeResponse>(responseText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null)
            {
                throw new Exception("Unable to deserialize scripture analysis model from AI response.");
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AIService] AnalyzeScriptureAsync Failure fallback active: {ex.Message}");
            return GenerateAnalyzeFallback(text);
        }
    }

    private ScriptureAnalyzeResponse GenerateAnalyzeFallback(string text)
    {
        // Try Archive matches first
        var archiveMatch = TryMatchingScriptureArchive(text);
        if (archiveMatch != null)
        {
            return archiveMatch with
            {
                SpiritualSignificance = $"{archiveMatch.SpiritualSignificance}\n\n⚠️ [Offline Scholar Engine Active] Analysis computed locally using built-in Vedic corpus archives. Key is resting or rate-limited.",
                IsFallback = true
            };
        }

        string iastVal = Transliterator.DevanagariToIast(text);
        string phoneticVal = Transliterator.IastToPhonetic(iastVal);
        var tokens = text.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        var elementsBreakdown = tokens.Select(ParseGenericWordBreakdown).Where(b => b != null).Cast<WordBreakdownItem>().ToList();

        return new ScriptureAnalyzeResponse(
            Verse: text,
            IdentifiedSource: "Vedic Classical Scripture (Heuristics Parsed)",
            TransliterationIAST: iastVal,
            TransliterationPhonetic: phoneticVal,
            TranslationEnglish: $"Rendered Translation (Roman Script): \"{iastVal}\"",
            TranslationHindi: $"देवनागरी लिप्यन्तरण: \"{text}\"",
            SpiritualSignificance: "⚠️ [Offline Scholar Engine Active] This verse has been parsed using offline fallback filters due to live AI service rate limit restrictions/configuration.\n\nPhonetic and character sandhis are mapped successfully. Live explanations will load once API keys refresh.",
            WordBreakdown: elementsBreakdown,
            PoeticMeter: tokens.Length >= 8 ? "Anustubh (Possibility)" : "Vedic Chanting Meter",
            IsFallback: true
        );
    }
}
