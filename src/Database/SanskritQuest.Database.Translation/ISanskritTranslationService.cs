using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SanskritQuest.Database.Translation
{
    public interface ISanskritTranslationService
    {
        Task<TranslationResult?> TranslateShlokaAsync(
            int index,
            string sanskritText,
            string? existingEnglish = null,
            CancellationToken cancellationToken = default,
            TranslationTarget? target = null);

        Task<List<TranslationResult>?> TranslateShlokasBatchAsync(
            List<(int index, string sanskrit, string? english)> batch,
            CancellationToken cancellationToken = default,
            TranslationTarget? target = null);
    }
}
