using PoLingual.Web.Services.Diagnostics;

namespace PoLingual.Web.Endpoints;

/// <summary>
/// Minimal API endpoints for diagnostics.
/// </summary>
public static class DiagnosticsEndpoints
{
    public static IEndpointRouteBuilder MapDiagnosticsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/diagnostics").WithTags("Diagnostics").WithOpenApi();

        group.MapGet("/", async (IDiagnosticsService diagnosticsService) =>
        {
            var results = await diagnosticsService.RunAllChecksAsync();
            return Results.Ok(results);
        })
        .WithName("RunDiagnostics")
        .WithSummary("Runs all diagnostics checks and returns results.");

        return endpoints;
    }
}
