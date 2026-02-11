using PoLingual.Web.Configuration;

namespace PoLingual.Web.Validators;

public class SpeechConfigValidator : ISpeechConfigValidator
{
    public bool IsValid(ApiSettings settings) =>
        !string.IsNullOrWhiteSpace(settings.AzureSpeechSubscriptionKey) &&
        !string.IsNullOrWhiteSpace(settings.AzureSpeechRegion);

    public string GetValidationError(ApiSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.AzureSpeechSubscriptionKey))
            return "AzureSpeechSubscriptionKey is not configured.";
        if (string.IsNullOrWhiteSpace(settings.AzureSpeechRegion))
            return "AzureSpeechRegion is not configured.";
        return string.Empty;
    }
}
