using PoLingual.Web.Services.Orchestration;

namespace PoLingual.Web.Endpoints;

/// <summary>
/// Minimal API endpoints for debate state queries.
/// Note: actual debate flow is handled via SignalR hub.
/// </summary>
public static class DebateEndpoints
{
    public static IEndpointRouteBuilder MapDebateEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/debate").WithTags("Debate").WithOpenApi();

        group.MapGet("/state", (IDebateOrchestrator orchestrator) =>
        {
            var state = orchestrator.CurrentState;
            return Results.Ok(new
            {
                state.CurrentTurn,
                state.IsDebateInProgress,
                state.IsDebateFinished,
                state.IsGeneratingTurn,
                Rapper1 = state.Rapper1.Name,
                Rapper2 = state.Rapper2.Name,
                Topic = state.Topic.Title,
                state.WinnerName,
                state.JudgeReasoning,
                state.ErrorMessage
            });
        })
        .WithName("GetDebateState")
        .WithSummary("Retrieves the current debate state.");

        group.MapPost("/reset", (IDebateOrchestrator orchestrator) =>
        {
            orchestrator.ResetDebate();
            return Results.Ok(new { message = "Debate reset." });
        })
        .WithName("ResetDebate")
        .WithSummary("Resets the current debate.");

        return endpoints;
    }
}
