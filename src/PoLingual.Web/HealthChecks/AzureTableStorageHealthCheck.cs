using Azure.Data.Tables;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PoLingual.Web.HealthChecks;

public class AzureTableStorageHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AzureTableStorageHealthCheck> _logger;

    public AzureTableStorageHealthCheck(IConfiguration configuration, ILogger<AzureTableStorageHealthCheck> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var connectionString = _configuration["Azure:TableStorageConnectionString"];
            if (string.IsNullOrWhiteSpace(connectionString))
                return HealthCheckResult.Degraded("Table Storage connection string not configured.");

            var serviceClient = new TableServiceClient(connectionString);
            await foreach (var _ in serviceClient.QueryAsync(cancellationToken: cancellationToken))
            {
                break; // Just verify connectivity
            }
            return HealthCheckResult.Healthy("Azure Table Storage is reachable.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Table Storage health check failed.");
            return HealthCheckResult.Unhealthy("Azure Table Storage is not reachable.", ex);
        }
    }
}
