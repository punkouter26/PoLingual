using Microsoft.Extensions.Diagnostics.HealthChecks;
using PoLingual.Web.Services.AI;

namespace PoLingual.Web.HealthChecks;

public class AzureOpenAIHealthCheck : IHealthCheck
{
    private readonly IAzureOpenAIService _service;
    public AzureOpenAIHealthCheck(IAzureOpenAIService service) => _service = service;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_service.IsConfigured
            ? HealthCheckResult.Healthy("Azure OpenAI is configured.")
            : HealthCheckResult.Degraded("Azure OpenAI is not configured."));
    }
}
