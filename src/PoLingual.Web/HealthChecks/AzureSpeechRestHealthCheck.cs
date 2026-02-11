using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using PoLingual.Web.Configuration;
using PoLingual.Web.Validators;

namespace PoLingual.Web.HealthChecks;

public class AzureSpeechRestHealthCheck : IHealthCheck
{
    private readonly ApiSettings _settings;
    private readonly ISpeechConfigValidator _validator;

    public AzureSpeechRestHealthCheck(IOptions<ApiSettings> settings, ISpeechConfigValidator validator)
    {
        _settings = settings.Value;
        _validator = validator;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_validator.IsValid(_settings)
            ? HealthCheckResult.Healthy("Azure Speech REST API is configured.")
            : HealthCheckResult.Degraded($"Azure Speech REST API not configured: {_validator.GetValidationError(_settings)}"));
    }
}
