using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace PoLingual.Web.Extensions;

/// <summary>
/// Configures OpenTelemetry tracing and metrics.
/// Azure Monitor integration is handled via Application Insights SDK.
/// </summary>
public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddPoLingualOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("PoLingual"))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            });

        return services;
    }
}
