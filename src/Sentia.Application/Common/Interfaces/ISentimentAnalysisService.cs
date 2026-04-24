using Sentia.Domain.Entities;

namespace Sentia.Application.Common.Interfaces;

public record SentimentResult(SentimentLabel Label, double Score);

public interface ISentimentAnalysisService
{
    Task<SentimentResult> AnalyzeAsync(string text, CancellationToken cancellationToken = default);
}
