namespace Amigo.Application.Services
{
    
    public interface ITranslationService
    {
       
        Task<Dictionary<string, string>> TranslateAsync(
            string text,
            IReadOnlyList<string> targetLanguageCodes,
            CancellationToken cancellationToken = default);

      
        Task<Dictionary<string, Dictionary<string, string>>> TranslateBatchAsync(
            IReadOnlyList<string> texts,
            IReadOnlyList<string> targetLanguageCodes,
            CancellationToken cancellationToken = default);
    }
}