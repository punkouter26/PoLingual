using PoLingual.Web.Services.Data;

namespace PoLingual.Web.Endpoints;

/// <summary>
/// Minimal API endpoints for rapper management.
/// </summary>
public static class RapperEndpoints
{
    public static IEndpointRouteBuilder MapRapperEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/rappers").WithTags("Rappers").WithOpenApi();

        group.MapGet("/", async (IRapperRepository repo, ILogger<Program> logger) =>
        {
            try
            {
                var rappers = await repo.GetAllRappersAsync();
                return Results.Ok(rappers);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to retrieve rappers from storage");
                return Results.Ok(Array.Empty<object>());
            }
        })
        .WithName("GetRappers")
        .WithSummary("Retrieves all available rappers with win/loss records.");

        return endpoints;
    }
}
