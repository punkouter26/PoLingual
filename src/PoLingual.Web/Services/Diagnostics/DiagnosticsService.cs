using Microsoft.Extensions.Diagnostics.HealthChecks;
using PoLingual.Shared.Models;

namespace PoLingual.Web.Services.Diagnostics;

/// <summary>
/// Diagnostics service using SRP — delegates actual health checking to IHealthCheck implementations.
/// </summary>
public class DiagnosticsService : IDiagnosticsService
{
    private readonly ILogger<DiagnosticsService> _logger;
    private readonly HealthCheckService _healthCheckService;

    public DiagnosticsService(ILogger<DiagnosticsService> logger, HealthCheckService healthCheckService)
    {
        _logger = logger;
        _healthCheckService = healthCheckService;
    }

    public async Task<List<DiagnosticResult>> RunAllChecksAsync()
    {
        var report = await _healthCheckService.CheckHealthAsync();
        return report.Entries.Select(entry =>
        {
            var isHealthy = entry.Value.Status == HealthStatus.Healthy;
            if (!isHealthy) _logger.LogWarning("{Check} failed: {Status}", entry.Key, entry.Value.Status);
            return new DiagnosticResult
            {
                CheckName = entry.Key,
                Success = isHealthy,
                Message = isHealthy ? $"{entry.Key} is healthy"
                    : $"{entry.Key} failed: {entry.Value.Description ?? entry.Value.Exception?.Message ?? "Unknown error"}"
            };
        }).ToList();
    }
}
