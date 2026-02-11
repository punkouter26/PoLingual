namespace PoLingual.Web.Configuration;

/// <summary>
/// Configuration settings for Azure services (used by Translation & AudioSynthesis features).
/// Maps from Key Vault secrets and appsettings.
/// </summary>
public sealed class ApiSettings
{
    public string AzureOpenAIEndpoint { get; set; } = string.Empty;
    public string AzureOpenAIDeploymentName { get; set; } = string.Empty;
    public string AzureOpenAIApiKey { get; set; } = string.Empty;
    public string AzureSpeechSubscriptionKey { get; set; } = string.Empty;
    public string AzureSpeechRegion { get; set; } = string.Empty;
}
