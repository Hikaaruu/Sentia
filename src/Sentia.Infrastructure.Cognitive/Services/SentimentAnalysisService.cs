using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Options;
using Sentia.Application.Common.Interfaces;
using Sentia.Domain.Entities;
using Sentia.Infrastructure.Cognitive.Options;

namespace Sentia.Infrastructure.Cognitive.Services;

public class SentimentAnalysisService : ISentimentAnalysisService
{
    private readonly TextAnalyticsClient _client;

    public SentimentAnalysisService(IOptions<AzureAiLanguageOptions> options)
    {
        var opts = options.Value;
        _client = new TextAnalyticsClient(
            new Uri(opts.Endpoint),
            new AzureKeyCredential(opts.ApiKey));
    }

    public async Task<SentimentResult> AnalyzeAsync(string text, CancellationToken cancellationToken = default)
    {
        var response = await _client.AnalyzeSentimentAsync(text, cancellationToken: cancellationToken);
        var documentSentiment = response.Value;

        var label = documentSentiment.Sentiment switch
        {
            TextSentiment.Positive => SentimentLabel.Positive,
            TextSentiment.Negative => SentimentLabel.Negative,
            _ => SentimentLabel.Neutral
        };

        var score = label switch
        {
            SentimentLabel.Positive => documentSentiment.ConfidenceScores.Positive,
            SentimentLabel.Negative => documentSentiment.ConfidenceScores.Negative,
            _ => documentSentiment.ConfidenceScores.Neutral
        };

        return new SentimentResult(label, score);
    }
}
