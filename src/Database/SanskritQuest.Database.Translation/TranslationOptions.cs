namespace SanskritQuest.Database.Translation
{
    public enum TranslationTarget
    {
        English,
        Hindi,
        Both
    }

    public class TranslationOptions
    {
        public required string GeminiApiKey { get; set; }
        public string ModelName { get; set; } = "gemini-1.5-flash";
        public TranslationTarget Target { get; set; } = TranslationTarget.Both;
        public float Temperature { get; set; } = 0.2f;
        public string ApiEndpointUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta/models";
    }
}
