using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PoLingual.Web.HealthChecks;

public class NewsApiHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;
    public NewsApiHealthCheck(IConfiguration configuration) => _configuration = configuration;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["NewsApi:ApiKey"];
        return Task.FromResult(!string.IsNullOrWhiteSpace(apiKey)
            ? HealthCheckResult.Healthy("NewsAPI key is configured.")
            : HealthCheckResult.Degraded("NewsAPI key is not configured. Using fallback topics."));
    }
}
