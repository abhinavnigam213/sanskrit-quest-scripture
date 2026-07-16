using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanskritQuest.Database.Tools.Repository
{
    public record HierarchyNode(
        int ScriptureId,
        int? ParentId,
        string LocalLabel,
        string NodeType,
        string TitlesJson,
        string DescriptionJson,
        int SequenceNumber
    );

    public record VerseEntry(
        int HierarchyId,
        string VerseNumber,
        string VerseType,
        string ContentSanskrit,
        string VerseDataJson,
        int SourceId,
        int SearchWeight,
        string MetaTagsJson
    );

    public interface IIngestionRepository
    {
        Task<int> EnsureHierarchyNodeAsync(HierarchyNode node);
        Task IngestVersesBatchAsync(List<VerseEntry> verses);
    }
}
