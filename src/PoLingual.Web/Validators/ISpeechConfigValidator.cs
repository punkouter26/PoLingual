using PoLingual.Web.Configuration;

namespace PoLingual.Web.Validators;

public interface ISpeechConfigValidator
{
    bool IsValid(ApiSettings settings);
    string GetValidationError(ApiSettings settings);
}
