using System.Collections.Generic;
using System.Threading.Tasks;
using SanskritQuest.Data.Contracts;

namespace SanskritQuest.Business.Contracts;

public interface IScriptureService
{
    List<Scripture> GetPopularScriptures();
}

public interface IDictionaryService
{
    object GetDictionaryData(string? word);
}

public interface ITranslationService
{
    Task<TranslationResponse> TranslateTextAsync(TranslationRequest request);
}

public interface ITransliterateService
{
    Task<TransliterateResponse> TransliterateTextAsync(TransliterateRequest request);
}

public interface IAnalyzeService
{
    Task<ScriptureAnalyzeResponse> AnalyzeScriptureAsync(AnalyzeRequest request);
}
