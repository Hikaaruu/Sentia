using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sentia.Application.Common.Interfaces;
using Sentia.Infrastructure.Cognitive.Options;
using Sentia.Infrastructure.Cognitive.Services;

namespace Sentia.Infrastructure.Cognitive;

public static class CognitiveServiceRegistration
{
    public static IServiceCollection AddCognitive(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AzureAiLanguageOptions>(
            configuration.GetSection(AzureAiLanguageOptions.SectionName));

        services.AddScoped<ISentimentAnalysisService, SentimentAnalysisService>();
        services.AddSingleton<ISentimentAnalysisQueue, SentimentAnalysisQueue>();

        return services;
    }
}
