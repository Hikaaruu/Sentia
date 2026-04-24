namespace Sentia.Infrastructure.Cognitive.Options;

public class AzureAiLanguageOptions
{
    public const string SectionName = "AzureAiLanguage";

    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}
