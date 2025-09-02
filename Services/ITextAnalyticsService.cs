namespace Fundamentos.Language.Summary.Services
{
    public interface ITextAnalyticsService
    {
        Task<List<string>> ExtractSummarizeAsync(string document);
        Task<List<string>> AbstractSummarizeAsync(string document);
    }
}
