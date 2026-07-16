using System.Threading.Tasks;

namespace SanskritQuest.Database.Tools.Ingestion
{
    public interface IScriptureIngester
    {
        string ScriptureCode { get; }
        Task IngestAsync(string filePath, int scriptureId, int sourceId, int batchSize);
    }
}
