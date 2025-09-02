
using Azure;
using Azure.AI.TextAnalytics;

namespace Fundamentos.Language.Summary.Services
{
    public class TextAnalyticsService : ITextAnalyticsService
    {
        private readonly Azure.AzureKeyCredential credentials;
        private readonly Uri endpoint;
        public TextAnalyticsService(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration["Language:Key"], nameof(configuration));
            ArgumentNullException.ThrowIfNull(configuration["Language:Endpoint"], nameof(configuration));

            credentials = new Azure.AzureKeyCredential(configuration["Language:Key"].ToString());
            endpoint = new Uri(configuration["Language:Endpoint"].ToString());
        }

        public async Task<List<string>> AbstractSummarizeAsync(string document)
        {
            TextAnalyticsClientOptions languageOptions = new() { DefaultLanguage = "pt", DefaultCountryHint = "br" };
            TextAnalyticsClient client = new(this.endpoint, this.credentials, languageOptions);
            AbstractiveSummarizeOptions options = new()
            {
                SentenceCount = 5, // aumentar para mais frases
                ModelVersion = "latest"  // modelo mais recente para melhor qualidade
            };

            AbstractiveSummarizeOperation operation = await client.AbstractiveSummarizeAsync(
                WaitUntil.Completed,
                new List<string> { document },
                options: options);

            var summaries = new List<string>();

            await foreach (var page in operation.GetValuesAsync())
            {
                var documentResult = page.FirstOrDefault();
                if (documentResult == null || documentResult.HasError)
                {
                    summaries.Add("Erro ao processar o texto.");
                    continue;
                }

                summaries.AddRange(documentResult.Summaries.Select(s => s.Text));
            }

            return summaries;
        }

        public async Task<List<string>> ExtractSummarizeAsync(string document)
        {
            TextAnalyticsClient client = new(this.endpoint, this.credentials, new TextAnalyticsClientOptions { DefaultLanguage = "pt", DefaultCountryHint = "br" });
            ExtractiveSummarizeOptions options = new()
            {
                MaxSentenceCount = 5,   // aumentar para mais frases
                ModelVersion = "latest"  // modelo mais recente para melhor qualidade
            };

            ExtractiveSummarizeOperation operation = await client.ExtractiveSummarizeAsync(
                WaitUntil.Completed,
                new List<string> { document },
                options: options);

            var summaries = new List<string>();

            await foreach (var page in operation.GetValuesAsync())
            {
                var documentResult = page.FirstOrDefault();
                if (documentResult == null || documentResult.HasError)
                {
                    summaries.Add("Erro ao processar o texto.");
                    continue;
                }

                summaries.AddRange(documentResult.Sentences.Select(s => s.Text));
            }

            return summaries;
        } 
    }
}
